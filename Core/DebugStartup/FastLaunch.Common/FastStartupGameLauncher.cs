#if NO_INGAME_BUILD
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Core.Utils;
using MonoMod.RuntimeDetour;
using ReLogic.Content.Sources;
using ReLogic.OS;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace FastLaunch.Common;

public static class FastStartupLauncher
{
    /// <summary><see langword="true"/> if the game was started as a multiplayer client.</summary>
    public static bool IsMPClient { get; private set; }
    /// <summary><see langword="true"/> if the game was started as the server.</summary>
    public static bool IsServer { get; private set; }
    /// <summary><see langword="true"/> if the game was started as a singleplayer client.</summary>
    public static bool IsSingleplayer { get; private set; }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ClientMain(string[] args, int clientIndex)
    {
        IsSingleplayer = clientIndex == 0;
        IsServer = clientIndex == -1;
        IsMPClient = clientIndex > 0;
        if (IsServer)
        {
            Console.WriteLine("Launched server ");
            TrySetTitle("AAServer");
        }
        else if (IsSingleplayer)
        {
            Console.WriteLine("Launched singleplayer");
            TrySetTitle("AASPClient");
        }
        else
        {
            Console.WriteLine("Launched as client: " + clientIndex);
            TrySetTitle($"AA{clientIndex}Client");
        }

        args ??= [];
        string? file = args.FirstOrDefault();
        if (!File.Exists(file))
        {
            Console.WriteLine($"Missing tml path in launch args, does not exist or is not accessible");
            try
            {
                file = File.ReadAllLines("tmlpath.txt")[0].Trim();
                Console.WriteLine($"Using tml path from tmlpath.txt");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("tmlpath.txt file not found");
                return;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Missing tml path in tmlpath.txt");
                return;
            }
        }
        string tmlInstallPath = Environment.CurrentDirectory = new FileInfo(file).Directory!.FullName;
        Console.WriteLine($"tModLoader.dll at: {file}");
        Console.WriteLine($"Current directory: {Environment.CurrentDirectory}");
        CorePatchRunner.DoRun(args);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ServerMain(string[] args)
    {
        ClientMain(args, -1);
    }


    private static void TrySetTitle(string title)
    {
        try
        {
            Console.Title = title;
        }
        catch
        {

        }
    }

    /*private static class AssetsCache
    {
        public static void AddDetours()
        {
            ilhooks.Add(new(typeof(Main).GetMethod("LoadContent", FINSTANCE), il =>
            {
                ILCursor c = new(il);
                if (!c.TryGotoNext(MoveType.Before, t => t.MatchNewobj<AssetSourceController>()))
                {
                    c.EmitDelegate((IContentSource[] sources) => (IContentSource[])[.. sources]);
                }
            }));
        }
        const string assetsFileName = "";
        public static async Task LoadAssets()
        {
            try
            {
                FileStream fs = new(assetsFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.RandomAccess);
                
            }
            catch (FileNotFoundException)
            {

            }
        }
    }*/
}

static class CorePatchRunner
{
    private const BindingFlags FINSTANCE = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private const BindingFlags FSTATIC = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    internal static void DoRun(string[] args)
    {
        applyingDetoursTask = ApplyDetours().ContinueWith(t => Console.WriteLine($"Finished applying detours in {detoursSW.Elapsed}"));

        string[] mainArgs = ["-console", .. args];
        if (FastStartupLauncher.IsServer)
            mainArgs = ["-server", .. args];
        typeof(ModLoader).Assembly.EntryPoint!.Invoke(null, [mainArgs]);
    }

    private static List<Hook> detours = new List<Hook>(10);
    private static List<ILHook> ilhooks = new List<ILHook>(10);
    private static Task applyingDetoursTask = null!;
    private static Stopwatch detoursSW = null!;

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    static Task ApplyDetours()
    {
        var task = new Task(static () =>
        {
            Assembly tmlAssembly = typeof(ModLoader).Assembly;
            detoursSW = Stopwatch.StartNew();

            Type amt = tmlAssembly.GetType("Terraria.ModLoader.Core.AssemblyManager")!;
            Type tpt = tmlAssembly.GetType("Terraria.Program")!;
            Type tmt = typeof(Main);
            Type mot = tmlAssembly.GetType("Terraria.ModLoader.Core.ModOrganizer")!;

            detours.Add(new Hook(amt.GetMethod("IsLoadable", FSTATIC)!,
                (Func<object, Type, bool> orig, object mod, Type type) => true, false));

            detours.Add(new Hook(amt.GetMethod("JITAssemblies", FSTATIC)!,
                (Action<IEnumerable<Assembly>, PreJITFilter> orig, IEnumerable<Assembly> assemblies, PreJITFilter filter) => { }, false));

            detours.Add(new Hook(tpt.GetMethod("ForceJITOnAssembly", FSTATIC)!,
                (Action<IEnumerable<Type>> orig, IEnumerable<Type> assemblies) => { }, applyByDefault: false));

            detours.Add(new Hook(tpt.GetMethod("ForceStaticInitializers", FSTATIC, [typeof(Assembly)])!,
                (Action<Assembly> orig, Assembly assemblies) => { }, applyByDefault: false));

            detours.Add(new Hook(tmt.GetMethod("LoadContent", FINSTANCE)!, static (Action<Main> orig, Main self) =>
            {
                if (applyingDetoursTask?.IsCompleted is false)
                {
                    Console.WriteLine("Waiting detours");
                    applyingDetoursTask.ConfigureAwait(false).GetAwaiter().GetResult();
                }
                orig(self);
            }, false));

            detours.Add(new Hook(tmt.GetMethod("DrawSplash", FINSTANCE)!, static (Action<Main, GameTime> orig, Main self, GameTime gameTime) =>
            {
                Console.WriteLine("Fast splash start");
                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < 900 && Terraria.Main.showSplash; i++)
                {
                    orig(self, gameTime);
                    Terraria.Main.Assets.TransferCompletedAssets();
                }
                sw.Stop();
                Console.WriteLine($"Fast DrawSplash time: {sw.Elapsed}");

            }, false));

            // to trigger recompilation
            detours.Add(new Hook(amt.GetMethod("GetLoadableTypes", FSTATIC, [amt.GetNestedType("ModLoadContext", FSTATIC | FINSTANCE)!, typeof(MetadataLoadContext)])!,
                (Func<object, MetadataLoadContext, IDictionary<Assembly, Type[]>> orig, object mod, MetadataLoadContext mlc) => { return orig(mod, mlc); }, false));

            if (FastStartupLauncher.IsServer)
            {
                detours.Add(new Hook(typeof(Main).GetMethod("Initialize", FINSTANCE)!, (Action<Main> orig, Main self) =>
                {
                    orig(self);
                    bool specifiedWorld = Program.LaunchParameters.TryGetValue("-kboserverworld", out string? selectedWorld);
                    if (specifiedWorld)
                    {
                        Main.LoadWorlds();
                        if (Main.WorldList.FirstOrDefault(t => t.Name == selectedWorld) is { } worldFile)
                        {
                            Main.ActiveWorldFileData = worldFile;
                            Console.WriteLine("AA SERVER STARTER: Selected world: " + worldFile.Name);
                        }
                        else
                        {
                            Console.WriteLine($"AA SERVER STARTER: Specified world '{specifiedWorld}' was not found.");
                        }
                    }
                }, false));
            }

            //AssetsCache.AddDetours();

            List<Action> actions = new List<Action>(64);

            for (int i = 0; i < detours.Count; i++)
                actions.Add(detours[i].Apply);
            for (int i = 0; i < ilhooks.Count; i++)
                actions.Add(ilhooks[i].Apply);

            Parallel.Invoke(actions.ToArray());
            detoursSW.Stop();
        }, TaskCreationOptions.LongRunning);
        task.Start();
        return task;
    }
}
#endif