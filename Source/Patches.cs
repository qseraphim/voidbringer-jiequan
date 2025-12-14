using System;
using System.Collections.Generic;
using HarmonyLib;
using I2.Loc;
using InControl.NativeDeviceProfiles;
using NineSolsAPI;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Collections.IEnumerator;
using static TormentedJiequan.PATH_LIST;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace TormentedJiequan;

    [HarmonyPatch]
    public class Patches {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BossGeneralState), nameof(BossGeneralState.OnAnimationMove))]
        private static void ChasingAttackEnhancer(ref BossGeneralState __instance, ref Vector3 delta) {
            if (SceneManager.GetActiveScene().name == PATH_LIST.BOSS_ROOM.SCENE_NAME) {
                if (__instance.GetStateType() == MonsterBase.States.AttackParrying1) //fix stagger knockback being excessive (v1.0.1)
                    delta.x *= 1.05f;                                           //not sure if this even works, it sometimes seems to
                else delta.x *= 1.2f;
            }
        }

        [HarmonyPrefix] //THANK YOU GREG YOU JUST SAVED ME LIKE 20 HOURS OF SUFFERING
        [HarmonyPatch(typeof(MonsterBase), "CheckInit")]
        private static void OnMonsterInit(ref MonsterBase __instance) {
            if (SceneManager.GetActiveScene().name == PATH_LIST.BOSS_ROOM.SCENE_NAME) {
                ApplyNewState(ref __instance);
            }
        }
        
        private static void ApplyNewState(ref MonsterBase __instance) {
            var states = __instance.transform.Find("States");
            var changePhase = states.GetComponentInChildren<BossPhaseChangeState>();
            var changePhase3 = Object.Instantiate(changePhase, states);
        
            changePhase3.exitState = MonsterBase.States.Engaging;
            var scriptableThing =
                changePhase3.stateTypeScriptable = ScriptableObject.CreateInstance<MonsterStateScriptable>();
            scriptableThing.overrideStateType = MonsterBase.States.Trolling;
            scriptableThing.stateName = changePhase.BindingAnimation;

            __instance.postureSystem.DieHandleingStates = new() {
                MonsterBase.States.BossAngry,
                MonsterBase.States.BossAngry,
                //MonsterBase.States.Trolling,      removed for p3 1hp survive bug fix (v1.0.2)
                MonsterBase.States.LastHit,
                MonsterBase.States.Dead,
            };
            __instance.postureSystem.GenerateCurrentDieHandleStacks();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BossGeneralState), nameof(BossGeneralState.OnStateEnter))]
        private static void Check(ref BossGeneralState __instance) {
            if (SceneManager.GetActiveScene().name == PATH_LIST.BOSS_ROOM.SCENE_NAME && __instance.GetStateType() == MonsterBase.States.Attack5) {
                TormentedJiequan.Instance.StartCoroutine(TormentedJiequan.StateInterruptCoroutine(0.4f, MonsterManager.Instance.ClosetMonster, MonsterBase.States.Attack5, MonsterBase.States.Attack14));
            }

            if (SceneManager.GetActiveScene().name == PATH_LIST.BOSS_ROOM.SCENE_NAME && __instance.GetStateType() == MonsterBase.States.Attack2) {
                var randomizer = Random.Range(0, 2);
                if (randomizer == 1) {
                    TormentedJiequan.Instance.StartCoroutine(TormentedJiequan.StateInterruptCoroutine(0.3f, MonsterManager.Instance.ClosetMonster, MonsterBase.States.Attack2, MonsterBase.States.Attack12));
                } else {
                    TormentedJiequan.Instance.StartCoroutine(TormentedJiequan.StateInterruptCoroutine(0.3f, MonsterManager.Instance.ClosetMonster, MonsterBase.States.Attack2, MonsterBase.States.Attack9));
                }
            }

            if (SceneManager.GetActiveScene().name == PATH_LIST.BOSS_ROOM.SCENE_NAME && __instance.GetStateType() == MonsterBase.States.Attack12) {
            TormentedJiequan.Instance.StartCoroutine(TormentedJiequan.StateInterruptCoroutine(0.2f, MonsterManager.Instance.ClosetMonster, MonsterBase.States.Attack12, MonsterBase.States.Attack9));
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DamageDealer), nameof(DamageDealer.DamageAmount), MethodType.Getter)]
        private static void DamageBuff(ref DamageDealer __instance, ref float __result) {
            if (SceneManager.GetActiveScene().name != PATH_LIST.BOSS_ROOM.SCENE_NAME) return;
            float unbuffedDamage = __result;

            float multiplierUniversal = PATH_LIST.DAMAGE_MULTIPLIER.UNIVERSAL;
            float multiplierCrimsonSmash = PATH_LIST.DAMAGE_MULTIPLIER.ONESHOT;

            if (MonsterManager.Instance.ClosetMonster.CurrentState == MonsterBase.States.Attack5) {
                __result *= multiplierCrimsonSmash;
            } else {
                __result *= multiplierUniversal;
            }
        }
    

    [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.GetTranslation))]
        private static void TormentedNameChange(string Term, ref string __result) {
            if (Term != "Characters/NameTag_JieChuan") return;
            __result = $"{__result}, the Bringer of Void";
    }
}