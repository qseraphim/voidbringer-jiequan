using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using NineSolsAPI;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static TormentedJiequan.PATH_LIST;
using Random = UnityEngine.Random;

namespace TormentedJiequan;

[BepInDependency(NineSolsAPICore.PluginGUID)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class TormentedJiequan : BaseUnityPlugin {

    private Harmony _harmony = null!;

    private ConfigEntry<bool> _enableDarkness = null!;
    private ConfigEntry<bool> _enableJiequanLight = null!;
    private void Awake() {
        Log.Init(Logger);
        RCGLifeCycle.DontDestroyForever(gameObject);
        _harmony = Harmony.CreateAndPatchAll(typeof(TormentedJiequan).Assembly);
        ToastManager.Toast($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!"); //toaster
        Log.Info("Mod is loaded!");
        SceneManager.sceneLoaded += OnSceneLoaded;

        _enableDarkness = Config.Bind("Options", "Lights Out!", false, "(REQUIRES RELOADING THE SCENE TO WORK! JUST PRESS <<Retry>> OR LEAVE AND REJOIN JIEQUAN'S FIGHT) Turns off the lights in Jiequan's room. Not recommended for first-time clears.");
        _enableJiequanLight = Config.Bind("Options", "Jiequan Tracking Light", false, "(REQUIRES RELOADING THE SCENE TO WORK! JUST PRESS <<Retry>> OR LEAVE AND REJOIN JIEQUAN'S FIGHT) Makes a light follow Jiequan around. Highly recommended when used with <<Lights Out!>>");
        Instance = this;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private static void HealthChanger() {
        var monster = MonsterManager.Instance.ClosetMonster;
        var baseHealth = AccessTools.FieldRefAccess<MonsterStat, float>("BaseHealthValue");
        baseHealth(monster.monsterStat) = 6000f;          
        monster.monsterStat.Phase2HealthRatio = 1 + 0.0666667f;
        monster.monsterStat.Phase3HealthRatio = 1 + 0.1333333f;
        monster.monsterStat.BossMemoryHealthScale = 2.5f;
        monster.postureSystem.CurrentHealthValue = 7500f;
    }
    
    private static IEnumerator WaitForBossAndInit() {
        while (!MonsterManager.Instance || !MonsterManager.Instance.ClosetMonster) {
            yield return null;
        }
        HealthChanger();
    }

    public static IEnumerator StateInterruptCoroutine(float duration, MonsterBase monster, MonsterBase.States cancelableState, MonsterBase.States newState) {
        while (monster.CurrentState != cancelableState) yield return null;
        float timeElapsed = 0f;
        while (timeElapsed <= duration) {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        int random = Random.Range(1, 11); //10 is 9 here so I put 11 to be 10
        if (random <= 3) {
            monster.ChangeState(monster.GetState(newState)); //this means 30% chance to cancel state and go into new state
        }
    }


    public static TormentedJiequan? Instance {
        get; 
        private set;
    }
        
    private _2dxFX_ColorChange _tj = null!;
    private PlayerLight _tjl = null!;

    private _2dxFX_ColorChange _weaponA = null!;
    private _2dxFX_ColorChange _weaponB = null!;
    private _2dxFX_ColorChange _weaponC = null!;
    private _2dxFX_ColorChange _weaponD = null!;
    private _2dxFX_ColorChange _teleportSmoke = null!;
    private _2dxFX_ColorChange _knifething0 = null!;
    private _2dxFX_ColorChange _knifething1 = null!;
    private _2dxFX_ColorChange _knifething2 = null!;
    private void ColorChanger() {
        var color = GameObject.Find(PATH_LIST.STEALTH_GAME_MONSTER.BOSS_PATH + "MonsterCore/Animator(Proxy)/Animator/Boss_JieChuan"); 
        _tj = color.AddComponent<_2dxFX_ColorChange>();
        _tjl = GameObject.Find("GameCore(Clone)/RCG LifeCycle/PPlayer/RotateProxy/SpriteHolder/PlayerLightmask").GetComponent<PlayerLight>();
        _tj._Saturation = 0f;
        _tj._ValueBrightness = 0.2f;

        var weaponAcolor = GameObject.Find(STEALTH_GAME_MONSTER.BOSS_PATH +
                                           "MonsterCore/Animator(Proxy)/Animator/Boss_JieChuan/Weapons/Weapon_A/Weapon");
        var weaponBcolor = GameObject.Find(STEALTH_GAME_MONSTER.BOSS_PATH +
                                           "MonsterCore/Animator(Proxy)/Animator/Boss_JieChuan/Weapons/Weapon_B/Weapon");
        var weaponCcolor = GameObject.Find(STEALTH_GAME_MONSTER.BOSS_PATH +
                                           "MonsterCore/Animator(Proxy)/Animator/Boss_JieChuan/Weapons/Weapon_C/Weapon");
        var weaponDcolor = GameObject.Find(STEALTH_GAME_MONSTER.BOSS_PATH +
                            "MonsterCore/Animator(Proxy)/Animator/Boss_JieChuan/Weapons/Weapon_D/Weapon");
        var smokeColor = GameObject.Find(STEALTH_GAME_MONSTER.BOSS_PATH +
                                         "MonsterCore/Animator(Proxy)/Animator/Boss_JieChuan/Effect/TeleportSmoke/Effect_TeleportSmoke");
        var knifeColor0 = GameObject.Find(STEALTH_GAME_MONSTER.BOSS_PATH +
                                         "MonsterCore/Animator(Proxy)/Animator/LogicRoot/Circle Shooter/Flying Weapon Path/View/Bullet/BulletSprite");
        var knifeColor1 = GameObject.Find(STEALTH_GAME_MONSTER.BOSS_PATH +
                                          "MonsterCore/Animator(Proxy)/Animator/LogicRoot/Circle Shooter/Flying Weapon Path (1)/View/Bullet/BulletSprite");
        var knifeColor2 = GameObject.Find(STEALTH_GAME_MONSTER.BOSS_PATH +
                                          "MonsterCore/Animator(Proxy)/Animator/LogicRoot/Circle Shooter/Flying Weapon Path (2)/View/Bullet/BulletSprite");
        
        
        _weaponA = weaponAcolor.AddComponent<_2dxFX_ColorChange>();
        _weaponB = weaponBcolor.AddComponent<_2dxFX_ColorChange>();
        _weaponC = weaponCcolor.AddComponent<_2dxFX_ColorChange>();
        _weaponD = weaponDcolor.AddComponent<_2dxFX_ColorChange>();
        _teleportSmoke = smokeColor.AddComponent<_2dxFX_ColorChange>();
        _knifething0 = knifeColor0.AddComponent<_2dxFX_ColorChange>();
        _knifething1 = knifeColor1.AddComponent<_2dxFX_ColorChange>();
        _knifething2 = knifeColor2.AddComponent<_2dxFX_ColorChange>();
        
        _weaponA._Saturation = 0f;
        _weaponB._Saturation = 0f;
        _weaponC._Saturation = 0f;
        _weaponD._Saturation = 0f;
        _teleportSmoke._Saturation = 0f;
        _knifething0._Saturation = 0f;
        _knifething1._Saturation = 0f;
        _knifething2._Saturation = 0f;
        
        _weaponA._ValueBrightness = 0.95f;
        _weaponB._ValueBrightness = 0.95f;
        _weaponC._ValueBrightness = 0.95f;
        _weaponD._ValueBrightness = 0.95f;
        _teleportSmoke._ValueBrightness = 0.2f;
        _knifething0._ValueBrightness = 0.95f;
        _knifething1._ValueBrightness = 0.95f;
        _knifething2._ValueBrightness = 0.95f;

        //notes:
        //Crimson visual spark - A5_S5/Room/EventBinder/General Boss Fight FSM Object_結權/FSM Animator/LogicRoot/---Boss---/BossShowHealthArea/StealthGameMonster_Boss_JieChuan/MonsterCore/Animator(Proxy)/Animator/LogicRoot/Circle Shooter/Flying Weapon Path/PathShootCore_Attack1/Docker/DangerHintEffect (1)

    }
    
    
    private void LightingHandler() {
        var postProcessVolume = GameObject.Find("A5_S5/CameraCore/DockObj/OffsetObj/ShakeObj/SceneCamera/AmplifyLightingSystem/FxCamera")
            .GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
        var lightsOut = GameObject.Find(PATH_LIST.BOSS_ROOM.ROOM_NAME + "RoomVibe");
        if (_enableDarkness.Value == true) {
            lightsOut.SetActive(false);
            postProcessVolume.weight = 0.1f;
        }
        if (_enableJiequanLight.Value == true) {
            Instantiate(_tjl.gameObject, _tj.transform);
        }

        var lastStandJadeTint = GameObject.Find("A5_S5/CameraCore/DockObj/OffsetObj/ShakeObj/SceneCamera/AmplifyLightingSystem/EffectCamera(請開著，Runtime會撈到SceneCamera做)");
        lastStandJadeTint.SetActive(false);
    }

    private LinkNextMoveStateWeight phase2DoubleSlashWeight = null!;
    private LinkNextMoveStateWeight phase2DaggersWeight = null!;
    private LinkNextMoveStateWeight phase2DiagonalWeight = null!;
    private LinkNextMoveStateWeight phase2PlungeWeight = null!;
    private LinkNextMoveStateWeight phase2StabWeight = null!;
    private LinkNextMoveStateWeight phase2GrenadesWeight = null!;
    private LinkNextMoveStateWeight phase2TeleportWeight = null!;
    private LinkNextMoveStateWeight phase2HorizontalWeight = null!;
    
    private void Phase3Enabler(){
        var doubleSlash = GameObject.Find(JIEQUAN_ATTACK.DOUBLE_SLASH);
        phase2DoubleSlashWeight = GameObject.Find(JIEQUAN_ATTACK.DOUBLE_SLASH + "/phase2")
            .GetComponent<LinkNextMoveStateWeight>();
        var phase3DoubleSlashWeight = Instantiate(phase2DoubleSlashWeight, doubleSlash.transform);
        phase3DoubleSlashWeight.name = "phase3";

        var daggers = GameObject.Find(JIEQUAN_ATTACK.DAGGERS);
        phase2DaggersWeight = GameObject.Find(JIEQUAN_ATTACK.DAGGERS + "/phase2")
            .GetComponent<LinkNextMoveStateWeight>();
        var phase3DaggersWeight = Instantiate(phase2DaggersWeight, daggers.transform);
        phase3DaggersWeight.name = "phase3";

        var diagonal = GameObject.Find(JIEQUAN_ATTACK.DIAGONAL);
        phase2DiagonalWeight = GameObject.Find(JIEQUAN_ATTACK.DIAGONAL + "/phase2")
            .GetComponent<LinkNextMoveStateWeight>();
        var phase3DiagonalWeight = Instantiate(phase2DiagonalWeight, diagonal.transform);
        phase3DiagonalWeight.name = "phase3";

        var plunge = GameObject.Find(JIEQUAN_ATTACK.PLUNGE);
        phase2PlungeWeight = GameObject.Find(JIEQUAN_ATTACK.PLUNGE + "/phase2")
            .GetComponent<LinkNextMoveStateWeight>();
        var phase3PlungeWeight = Instantiate(phase2PlungeWeight, plunge.transform);
        phase3PlungeWeight.name = "phase3";

        var stab = GameObject.Find(JIEQUAN_ATTACK.STAB);
        phase2StabWeight = GameObject.Find(JIEQUAN_ATTACK.STAB + "/phase2")
            .GetComponent<LinkNextMoveStateWeight>();
        var phase3StabWeight = Instantiate(phase2StabWeight, stab.transform);
        phase3StabWeight.name = "phase3";

        var grenades = GameObject.Find(JIEQUAN_ATTACK.GRENADES);
        phase2GrenadesWeight = GameObject.Find(JIEQUAN_ATTACK.STAB + "/phase2")
            .GetComponent<LinkNextMoveStateWeight>();
        var phase3GrenadesWeight = Instantiate(phase2GrenadesWeight, grenades.transform);
        phase3GrenadesWeight.name = "phase3";

        var teleport = GameObject.Find(JIEQUAN_ATTACK.TELEPORT);
        phase2TeleportWeight = GameObject.Find(JIEQUAN_ATTACK.TELEPORT + "/phase2")
            .GetComponent<LinkNextMoveStateWeight>();
        var phase3TeleportWeight = Instantiate(phase2TeleportWeight, teleport.transform);
        phase3TeleportWeight.name = "phase3";

        var horizontal = GameObject.Find(JIEQUAN_ATTACK.LINK_REVERSE_STAB);
        phase2HorizontalWeight = GameObject.Find(JIEQUAN_ATTACK.LINK_REVERSE_STAB + "/phase2")
            .GetComponent<LinkNextMoveStateWeight>();
        var phase3HorizontalWeight = Instantiate(phase2HorizontalWeight, horizontal.transform);
        phase3HorizontalWeight.name = "phase3";
    }

    private void WeightChangerP3() {
        var weightsDoubleSlashP3 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH + "/phase3")
                .GetComponent<LinkNextMoveStateWeight>();

        weightsDoubleSlashP3.stateWeightList.Clear();
        weightsDoubleSlashP3.mustUseStates.Clear();
            
            AttackWeight p3Doubleslashweight1 = new AttackWeight();
            p3Doubleslashweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_THRUST)).GetComponent<BossGeneralState>();
            p3Doubleslashweight1.weight = 1;
            weightsDoubleSlashP3.stateWeightList.Add(p3Doubleslashweight1);
            
            AttackWeight p3Doubleslashweight2 = new AttackWeight();
            p3Doubleslashweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DIAGONAL)).GetComponent<BossGeneralState>();
            p3Doubleslashweight2.weight = 1;
            weightsDoubleSlashP3.stateWeightList.Add(p3Doubleslashweight2);
            
            AttackWeight p3Doubleslashweight3 = new AttackWeight();
            p3Doubleslashweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT)).GetComponent<BossGeneralState>();
            p3Doubleslashweight3.weight = 1;
            weightsDoubleSlashP3.stateWeightList.Add(p3Doubleslashweight3);
            
            AttackWeight p3Doubleslashweight4 = new AttackWeight();
            p3Doubleslashweight4.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH)).GetComponent<BossGeneralState>();
            p3Doubleslashweight4.weight = 1;
            weightsDoubleSlashP3.stateWeightList.Add(p3Doubleslashweight4);
            
            AttackWeight p3Doubleslashweight5 = new AttackWeight();
            p3Doubleslashweight5.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.PLUNGE)).GetComponent<BossGeneralState>();
            p3Doubleslashweight5.weight = 1;
            weightsDoubleSlashP3.stateWeightList.Add(p3Doubleslashweight5);
            
            AttackWeight p3Doubleslashweight6 = new AttackWeight();
            p3Doubleslashweight6.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.HORIZONTAL_THRUST)).GetComponent<BossGeneralState>();
            p3Doubleslashweight6.weight = 1;
            weightsDoubleSlashP3.stateWeightList.Add(p3Doubleslashweight6);
            
            var weightStabP3 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.STAB + "/phase3")
                .GetComponent<LinkNextMoveStateWeight>();
            
            weightStabP3.stateWeightList.Clear();
            weightStabP3.mustUseStates.Clear();

            AttackWeight p3Stabweight1 = new AttackWeight();
            p3Stabweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.HORIZONTAL_THRUST)).GetComponent<BossGeneralState>();
            p3Stabweight1.weight = 1;
            weightStabP3.stateWeightList.Add(p3Stabweight1);

            AttackWeight p3Stabweight2 = new AttackWeight();
            p3Stabweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_SMASH)).GetComponent<BossGeneralState>();
            p3Stabweight2.weight = 1;
            weightStabP3.stateWeightList.Add(p3Stabweight2);
            
            var weightPlungeP3 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.PLUNGE + "/phase3")
                .GetComponent<LinkNextMoveStateWeight>();
            weightPlungeP3.stateWeightList.Clear();
            weightPlungeP3.mustUseStates.Clear();
            
            AttackWeight p3Plungeweight1 = new AttackWeight(); 
            p3Plungeweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.STAB)).GetComponent<BossGeneralState>();
            p3Plungeweight1.weight = 1;
            weightPlungeP3.stateWeightList.Add(p3Plungeweight1);

            AttackWeight p3Plungeweight2 = new AttackWeight();
            p3Plungeweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT)).GetComponent<BossGeneralState>();
            p3Plungeweight2.weight = 1;
            weightPlungeP3.stateWeightList.Add(p3Plungeweight2);

            AttackWeight p3Plungeweight3 = new AttackWeight();
            p3Plungeweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.SHIELD)).GetComponent<BossGeneralState>();
            p3Plungeweight3.weight = 1;
            weightPlungeP3.stateWeightList.Add(p3Plungeweight3);
            
            var weightTeleportP3 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT + "/phase3")
                .GetComponent<LinkNextMoveStateWeight>();
            
            AttackWeight p3Teleportweight1 = new AttackWeight();
            p3Teleportweight1.state = 
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT)).GetComponent<BossGeneralState>();
            p3Teleportweight1.weight = 1;
            weightTeleportP3.stateWeightList.Add(p3Teleportweight1);

            AttackWeight p3Teleportweight2 = new AttackWeight();
            p3Teleportweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH)).GetComponent<BossGeneralState>();
            p3Teleportweight2.weight = 1;
            weightTeleportP3.stateWeightList.Add(p3Teleportweight2);

            AttackWeight p3Teleportweight3 = new AttackWeight();
            p3Teleportweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_SMASH)).GetComponent<BossGeneralState>();
            p3Teleportweight3.weight = 1;
            weightTeleportP3.stateWeightList.Add(p3Teleportweight3);
            
            AttackWeight p3Teleportweight4 = new AttackWeight();
            p3Teleportweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_THRUST)).GetComponent<BossGeneralState>();
            p3Teleportweight3.weight = 1;
            weightTeleportP3.stateWeightList.Add(p3Teleportweight4);
            
            AttackWeight p3Teleportweight5 = new AttackWeight();
            p3Teleportweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.GRENADES)).GetComponent<BossGeneralState>();
            p3Teleportweight3.weight = 1;
            weightTeleportP3.stateWeightList.Add(p3Teleportweight5);
            
            var weightDiagonalP3 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DIAGONAL + "/phase3")
                .GetComponent<LinkNextMoveStateWeight>();
            
            weightDiagonalP3.stateWeightList.Clear();
            weightDiagonalP3.mustUseStates.Clear();
            
            AttackWeight p3Diagonalweight1 = new AttackWeight();
            p3Diagonalweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_SMASH)).GetComponent<BossGeneralState>();
            p3Diagonalweight1.weight = 1;
            weightDiagonalP3.stateWeightList.Add(p3Diagonalweight1);
            
            AttackWeight p3Diagonalweight2 = new AttackWeight();
            p3Diagonalweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_SMASH)).GetComponent<BossGeneralState>();
            p3Diagonalweight2.weight = 1;
            weightDiagonalP3.stateWeightList.Add(p3Diagonalweight2);
            
            AttackWeight p3Diagonalweight3 = new AttackWeight();
            p3Diagonalweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_SMASH)).GetComponent<BossGeneralState>();
            p3Diagonalweight3.weight = 1;
            weightDiagonalP3.stateWeightList.Add(p3Diagonalweight3);
            
            var weightHorizontalP3 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.LINK_REVERSE_STAB + "/phase3")
                .GetComponent<LinkNextMoveStateWeight>();

            weightHorizontalP3.stateWeightList.Clear();
            weightHorizontalP3.mustUseStates.Clear();

            AttackWeight p3Horizontalweight1 = new AttackWeight();
            p3Horizontalweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.LINK_REVERSE_STAB)).GetComponent<BossGeneralState>();
            p3Horizontalweight1.weight = 1;
            weightHorizontalP3.stateWeightList.Add(p3Horizontalweight1);
            
            AttackWeight p3Horizontalweight2 = new AttackWeight();
            p3Horizontalweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS)).GetComponent<BossGeneralState>();
            p3Horizontalweight2.weight = 1;
            weightHorizontalP3.stateWeightList.Add(p3Horizontalweight2);
            
            AttackWeight p3Horizontalweight3 = new AttackWeight();
            p3Horizontalweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.SHIELD)).GetComponent<BossGeneralState>();
            p3Horizontalweight3.weight = 1;
            weightHorizontalP3.stateWeightList.Add(p3Horizontalweight3);
            
            AttackWeight p3Horizontalweight4 = new AttackWeight();
            p3Horizontalweight4.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_SMASH)).GetComponent<BossGeneralState>();
            p3Horizontalweight4.weight = 1;
            weightHorizontalP3.stateWeightList.Add(p3Horizontalweight4);
            
            var weightGrenadesP3 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.GRENADES + "/phase3")
                .GetComponent<LinkNextMoveStateWeight>();
            weightGrenadesP3.stateWeightList.Clear();

            AttackWeight p3Grenadesweight1 = new AttackWeight();
            p3Grenadesweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS)).GetComponent<BossGeneralState>();
            p3Grenadesweight1.weight = 1;
            weightGrenadesP3.stateWeightList.Add(p3Grenadesweight1);
            
            var weightDaggersP3 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS + "/phase3")
                .GetComponent<LinkNextMoveStateWeight>();
   
            weightDaggersP3.stateWeightList.Clear();
            weightDaggersP3.mustUseStates.Clear();

            AttackWeight p3Daggersweight1 = new AttackWeight();
            p3Daggersweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS)).GetComponent<BossGeneralState>();
            p3Daggersweight1.weight = 1;
            weightDaggersP3.stateWeightList.Add(p3Daggersweight1);

            AttackWeight p3Daggersweight2 = new AttackWeight();
            p3Daggersweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH)).GetComponent<BossGeneralState>();
            p3Daggersweight2.weight = 1;
            weightDaggersP3.stateWeightList.Add(p3Daggersweight2);
    }
    private static void JiDecoration() {
        var ji = GameObject.Find(PATH_LIST.BOSS_ROOM.ROOM_NAME + "DressingSpace/Jee_Chill");
        ji.SetActive(true);

        var jiEyeDarkness = GameObject.Find(PATH_LIST.BOSS_ROOM.ROOM_NAME + "DressingSpace/Jee_Chill/Jee/Track_Eye");
        jiEyeDarkness.SetActive(false);
    }

    private static void StaggerRemover() {
        var fullControlStagger = GameObject.Find(PATH_LIST.JIEQUAN_STATE.STAGGER);
        fullControlStagger.SetActive(false);
        var shieldBreakStagger = GameObject.Find(PATH_LIST.JIEQUAN_STATE.SHIELD_BREAK_STAGGER);
        shieldBreakStagger.SetActive(false);
        var unboundCounterStagger = GameObject.Find(PATH_LIST.JIEQUAN_STATE.UC_STAGGER);
        unboundCounterStagger.SetActive(false);
    }
    
    private static void AttackSpeedChanger() {
                var doubleSlashSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH))
                    .GetComponent<BossGeneralState>();
                doubleSlashSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.DOUBLE_SLASH_SPEED;
                doubleSlashSpeed.OverideAnimationSpeed = true;
                
                var daggersSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS))
                    .GetComponent<BossGeneralState>();
                daggersSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.DAGGERS_SPEED;
                daggersSpeed.OverideAnimationSpeed = true;
    
                var diagonalSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DIAGONAL))
                    .GetComponent<BossGeneralState>();
                diagonalSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.DIAGONAL_SPEED;
                diagonalSpeed.OverideAnimationSpeed = true;
    
                var plungeSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.PLUNGE))
                    .GetComponent<BossGeneralState>();
                plungeSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.PLUNGE_SPEED;
                plungeSpeed.OverideAnimationSpeed = true;
    
                var crimsonSmashSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_SMASH))
                    .GetComponent<BossGeneralState>();
                crimsonSmashSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.CRIMSON_SMASH_SPEED;
                crimsonSmashSpeed.OverideAnimationSpeed = true;
    
                var stabSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.STAB))
                    .GetComponent<BossGeneralState>();
                stabSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.STAB_SPEED;
                stabSpeed.OverideAnimationSpeed = true;
    
                var grenadesSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.GRENADES))
                    .GetComponent<BossGeneralState>();
                grenadesSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.GRENADES_SPEED;
                grenadesSpeed.OverideAnimationSpeed = true;
    
                var shieldSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.SHIELD))
                    .GetComponent<BossGeneralState>();
                shieldSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.SHIELD_SPEED;
                shieldSpeed.OverideAnimationSpeed = true;
    
                var teleportSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT))
                    .GetComponent<BossGeneralState>();
                teleportSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.TELEPORT_SPEED;
                teleportSpeed.OverideAnimationSpeed = true;
                
                var horizontalSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.HORIZONTAL_THRUST))
                    .GetComponent<BossGeneralState>();
                horizontalSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.HORIZONTAL_SPEED;
                horizontalSpeed.OverideAnimationSpeed = true;
                
                var linkRevSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.LINK_REVERSE_STAB))
                    .GetComponent<BossGeneralState>();
                linkRevSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.HORIZONTAL_SPEED;
                linkRevSpeed.OverideAnimationSpeed = true;
                
                var crimsonThrustSpeed = (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_THRUST))
                    .GetComponent<BossGeneralState>();
                crimsonThrustSpeed.AnimationSpeed = PATH_LIST.SPEED_MULTIPLIER.CRIMSON_THRUST_SPEED;
                crimsonThrustSpeed.OverideAnimationSpeed = true;
        }
    
    private static void WeightChangerP1() {
        var weightsDoubleSlashP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH + "/phase1")
                .GetComponent<LinkNextMoveStateWeight>();

        weightsDoubleSlashP1.stateWeightList.Clear();
        weightsDoubleSlashP1.mustUseStates.Clear(); //cringe deterministic selector
            
            AttackWeight p1Doubleslashweight1 = new AttackWeight();
            p1Doubleslashweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_THRUST)).GetComponent<BossGeneralState>();
            p1Doubleslashweight1.weight = 1;
            weightsDoubleSlashP1.stateWeightList.Add(p1Doubleslashweight1);
            
            AttackWeight p1Doubleslashweight2 = new AttackWeight();
            p1Doubleslashweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_SMASH)).GetComponent<BossGeneralState>();
            p1Doubleslashweight2.weight = 1;
            weightsDoubleSlashP1.stateWeightList.Add(p1Doubleslashweight2);
            
            AttackWeight p1Doubleslashweight3 = new AttackWeight();
            p1Doubleslashweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT)).GetComponent<BossGeneralState>();
            p1Doubleslashweight3.weight = 1;
            weightsDoubleSlashP1.stateWeightList.Add(p1Doubleslashweight3);
            
            AttackWeight p1Doubleslashweight4 = new AttackWeight();
            p1Doubleslashweight4.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH)).GetComponent<BossGeneralState>();
            p1Doubleslashweight4.weight = 1;
            weightsDoubleSlashP1.stateWeightList.Add(p1Doubleslashweight4);
            
            var weightStabP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.STAB + "/phase1")
                .GetComponent<LinkNextMoveStateWeight>();
            
            weightStabP1.stateWeightList.Clear();

            AttackWeight p1Stabweight1 = new AttackWeight();
            p1Stabweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.PLUNGE)).GetComponent<BossGeneralState>();
            p1Stabweight1.weight = 1;
            weightStabP1.stateWeightList.Add(p1Stabweight1);

            AttackWeight p1Stabweight2 = new AttackWeight();
            p1Stabweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH)).GetComponent<BossGeneralState>();
            p1Stabweight2.weight = 1;
            weightStabP1.stateWeightList.Add(p1Stabweight2);

            AttackWeight p1Stabweight3 = new AttackWeight();
            p1Stabweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT)).GetComponent<BossGeneralState>();
            p1Stabweight3.weight = 1;
            weightStabP1.stateWeightList.Add(p1Stabweight3);

            var weightCrimsonThrustP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.CRIMSON_THRUST + "/weight")
                .GetComponent<LinkNextMoveStateWeight>();
            
            weightCrimsonThrustP1.mustUseStates.Clear();

            AttackWeight p1Crimsonthrustweight1 = new AttackWeight();
            p1Crimsonthrustweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.PLUNGE)).GetComponent<BossGeneralState>();
            p1Crimsonthrustweight1.weight = 1;
            weightCrimsonThrustP1.stateWeightList.Add(p1Crimsonthrustweight1);

            AttackWeight p1Crimsonthrustweight2 = new AttackWeight();
            p1Crimsonthrustweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS)).GetComponent<BossGeneralState>();
            p1Crimsonthrustweight2.weight = 1;
            weightCrimsonThrustP1.stateWeightList.Add(p1Crimsonthrustweight2);
            
            var weightPlungeP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.PLUNGE + "/phase1")
                .GetComponent<LinkNextMoveStateWeight>();
            weightPlungeP1.stateWeightList.Clear();
            
            AttackWeight p1Plungeweight1 = new AttackWeight(); 
            p1Plungeweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DIAGONAL)).GetComponent<BossGeneralState>();
            p1Plungeweight1.weight = 2;
            weightPlungeP1.stateWeightList.Add(p1Plungeweight1);
            
            var weightTeleportP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT + "/phase1")
                .GetComponent<LinkNextMoveStateWeight>();
            AttackWeight p1Teleportweight1 = new AttackWeight();
            p1Teleportweight1.state = 
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.TELEPORT)).GetComponent<BossGeneralState>();
            p1Teleportweight1.weight = 1;
            weightTeleportP1.stateWeightList.Add(p1Teleportweight1);

            AttackWeight p1Teleportweight2 = new AttackWeight();
            p1Teleportweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DOUBLE_SLASH)).GetComponent<BossGeneralState>();
            p1Teleportweight2.weight = 1;
            weightTeleportP1.stateWeightList.Add(p1Teleportweight2);

            AttackWeight p1Teleportweight3 = new AttackWeight();
            p1Teleportweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.STAB)).GetComponent<BossGeneralState>();
            p1Teleportweight3.weight = 1;
            weightTeleportP1.stateWeightList.Add(p1Teleportweight3);
            
            var weightDiagonalP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DIAGONAL + "/phase1")
                .GetComponent<LinkNextMoveStateWeight>();
            
            weightDiagonalP1.stateWeightList.Clear();
            
            AttackWeight p1Diagonalweight1 = new AttackWeight();
            p1Diagonalweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.LINK_REVERSE_STAB)).GetComponent<BossGeneralState>();
            p1Diagonalweight1.weight = 1;
            weightDiagonalP1.stateWeightList.Add(p1Diagonalweight1);
            AttackWeight p1Diagonalweight2 = new AttackWeight();
            p1Diagonalweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.LINK_REVERSE_STAB)).GetComponent<BossGeneralState>();
            p1Diagonalweight2.weight = 1;
            weightDiagonalP1.stateWeightList.Add(p1Diagonalweight2);
            AttackWeight p1Diagonalweight3 = new AttackWeight();
            p1Diagonalweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.LINK_REVERSE_STAB)).GetComponent<BossGeneralState>();
            p1Diagonalweight3.weight = 1;
            weightDiagonalP1.stateWeightList.Add(p1Diagonalweight3);
            
            var weightHorizontalP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.LINK_REVERSE_STAB + "/phase1")
                .GetComponent<LinkNextMoveStateWeight>();

            weightHorizontalP1.stateWeightList.Clear();

            AttackWeight p1Horizontalweight1 = new AttackWeight();
            p1Horizontalweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.GRENADES)).GetComponent<BossGeneralState>();
            p1Horizontalweight1.weight = 1;
            weightHorizontalP1.stateWeightList.Add(p1Horizontalweight1);
            AttackWeight p1Horizontalweight2 = new AttackWeight();
            p1Horizontalweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.GRENADES)).GetComponent<BossGeneralState>();
            p1Horizontalweight2.weight = 1;
            weightHorizontalP1.stateWeightList.Add(p1Horizontalweight2);
            
            var weightGrenadesP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.GRENADES + "/phase1")
                .GetComponent<LinkNextMoveStateWeight>();
            weightGrenadesP1.stateWeightList.Clear();

            AttackWeight p1Grenadesweight1 = new AttackWeight();
            p1Grenadesweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.GRENADES)).GetComponent<BossGeneralState>();
            p1Grenadesweight1.weight = 1;
            weightGrenadesP1.stateWeightList.Add(p1Grenadesweight1);

            AttackWeight p1Grenadesweight2 = new AttackWeight();
            p1Grenadesweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS)).GetComponent<BossGeneralState>();
            p1Grenadesweight2.weight = 1;
            weightGrenadesP1.stateWeightList.Add(p1Grenadesweight2);

            AttackWeight p1Grenadesweight3 = new AttackWeight();
            p1Grenadesweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.SHIELD)).GetComponent<BossGeneralState>();
            p1Grenadesweight3.weight = 1;
            weightGrenadesP1.stateWeightList.Add(p1Grenadesweight3);
            
            var weightDaggersP1 = GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS + "/phase1 ").GetComponent<LinkNextMoveStateWeight>();
            //RedCandle, why is there an empty space on this object's name? you guys are CRUEL I've spent
            //so many hours just trying to figure out why the GameObject.Find was returning null...

            weightDaggersP1.stateWeightList.Clear();

            AttackWeight p1Daggersweight1 = new AttackWeight();
            p1Daggersweight1.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.GRENADES)).GetComponent<BossGeneralState>();
            p1Daggersweight1.weight = 1;
            weightDaggersP1.stateWeightList.Add(p1Daggersweight1);

            AttackWeight p1Daggersweight2 = new AttackWeight();
            p1Daggersweight2.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.DAGGERS)).GetComponent<BossGeneralState>();
            p1Daggersweight2.weight = 1;
            weightDaggersP1.stateWeightList.Add(p1Daggersweight2);

            AttackWeight p1Daggersweight3 = new AttackWeight();
            p1Daggersweight3.state =
                (GameObject.Find(PATH_LIST.JIEQUAN_ATTACK.STAB)).GetComponent<BossGeneralState>();
            p1Daggersweight3.weight = 1;
            weightDaggersP1.stateWeightList.Add(p1Daggersweight3);

            //Unfortunately, Crimson Smash does not work with weights, it's essentially a forced combo finisher. Believe me, I tried adding a LinkNextMoveStateWeight and initialize some weight lists, but
            //it didn't end up doing anything, it has a mind of its own.
            //Crimson Smash can chain into a random choice from the default "starter attacks" selection
            //Which is quite unfortunate because Diagonal precedes Instant Horizontal Thrust...
            //Maybe that's not such a bad thing though
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == PATH_LIST.BOSS_ROOM.SCENE_NAME)
        {
            StartCoroutine(WaitForBossAndInit());
            
            ColorChanger();
            JiDecoration();
            LightingHandler();
            StaggerRemover();
            AttackSpeedChanger();
            WeightChangerP1();
            Phase3Enabler();
            WeightChangerP3();
        }
    }

    private void OnDestroy() {
        _harmony.UnpatchSelf();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }
}