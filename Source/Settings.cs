namespace TormentedJiequan;
using UnityEngine;
using BlendModes;

public static class PATH_LIST
{
    public static class BOSS_ROOM {
        public const string ROOM_NAME = "A5_S5/Room/EventBinder/General Boss Fight FSM Object_結權/JieChuanRoom/";
        public const string SCENE_NAME = "A5_S5_JieChuanHall";
    }

    public static class STEALTH_GAME_MONSTER {
        public const string BOSS_PATH = "A5_S5/Room/EventBinder/General Boss Fight FSM Object_結權/FSM Animator/LogicRoot/---Boss---/BossShowHealthArea/StealthGameMonster_Boss_JieChuan/";
        public const string STATES = BOSS_PATH + "States/";
        public const string ATTACKS = STATES + "Attacks/";
    }

    public static class JIEQUAN_STATE {
        public const string STAGGER = STEALTH_GAME_MONSTER.STATES + "Hurt_BigState";
        public const string SHIELD_BREAK_STAGGER = STEALTH_GAME_MONSTER.STATES + "PostureBreak";
        public const string UC_STAGGER = STEALTH_GAME_MONSTER.STATES + "AttackParrying";
        public const string PHASE_2_TRANSITION_STATE = STEALTH_GAME_MONSTER.STATES + "[BossAngry] BossAngry Phase Change";
        public const string PHASE_3_TRANSITION_STATE = STEALTH_GAME_MONSTER.STATES + "[BossAngry] BossAngry Phase Change(Clone)";
    }
    
    public static class JIEQUAN_ATTACK {
        public const string DOUBLE_SLASH = STEALTH_GAME_MONSTER.ATTACKS + "[1]TrippleSlashAndBack"; //spear slash up/down
        public const string DAGGERS = STEALTH_GAME_MONSTER.ATTACKS + "[2]Shoot"; //3 daggers
        public const string DIAGONAL = STEALTH_GAME_MONSTER.ATTACKS + "[3]ThrustAttack Through"; //spear diagonal thrust
        public const string PLUNGE = STEALTH_GAME_MONSTER.ATTACKS + "[4]FlipJumpAttack"; //halberd plunge attack
        public const string CRIMSON_SMASH = STEALTH_GAME_MONSTER.ATTACKS + "[5]DangerLargeSmash"; //spear crimson smash
        public const string STAB = STEALTH_GAME_MONSTER.ATTACKS + "[6]BackStab"; //sword stab
        public const string GRENADES = STEALTH_GAME_MONSTER.ATTACKS + "[7]ThrowGrenade"; //bombs
        public const string SHIELD = STEALTH_GAME_MONSTER.ATTACKS + "[8]DrinkPotion"; //shield and heal
        public const string TELEPORT = STEALTH_GAME_MONSTER.ATTACKS + "[9] TeleportToPlayerBack"; //phase 2 teleport
        public const string HORIZONTAL_THRUST = STEALTH_GAME_MONSTER.ATTACKS + "[11] Thurst Interrupt"; //fake spear horizontal thrust, it's not performable from my experience
        public const string CRIMSON_THRUST = STEALTH_GAME_MONSTER.ATTACKS + "[12]DangerLargeThrust"; //spear crimson thrust
        public const string LINK_REVERSE_STAB = STEALTH_GAME_MONSTER.ATTACKS + "[14]LinkReverseStab"; //the real horizontal thrust attack state
    }
    
    public static class SPEED_MULTIPLIER {
        public const float DOUBLE_SLASH_SPEED = 1 + 0.7f;
        public const float DAGGERS_SPEED = 1 + 0.7f;
        public const float DIAGONAL_SPEED = 1 + 0.8f;
        public const float PLUNGE_SPEED = 1 + 0.7f;
        public const float CRIMSON_SMASH_SPEED = 1 + 0.8f;
        public const float STAB_SPEED = 1 + 0.7f;
        public const float GRENADES_SPEED = 1 + 0.7f;
        public const float SHIELD_SPEED = 1 + 1f;
        public const float TELEPORT_SPEED = 1 + 2f;
        public const float HORIZONTAL_SPEED = 1 + 0.8f;
        public const float CRIMSON_THRUST_SPEED = 1 + 0.7f;
        public const float PHASE_2_TRANSITION_SPEED = 1 + 3f;
        public const float PHASE_3_TRANSITION_SPEED = 1 + 7f;
    }

    public static class DAMAGE_MULTIPLIER {
        public const float UNIVERSAL = 1.5f;
        public const float ONESHOT = 1 + 7.77f;
    }
}