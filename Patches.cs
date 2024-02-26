using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.TextCore;

namespace Tweaks
{
    internal class Patches
    {
        [HarmonyPatch(typeof(Character), "Hit")]
        class Character_Hit_Patch
        {
            public static void Prefix(Character __instance, HitData hitData)
            {
                //string s = "Character Hit " + __instance.name + ", damage " + hitData.damage.damage;
                hitData.damage.damage *= Config.playerDamageMult.Value;
                //DungeonEscapeUI.instance.ShowPopupMessage(s, duration: 1, fadeInSpeed: 11f, fadeSpeed: 11f);
                //Main.log.LogInfo(s);
            }
        }


        [HarmonyPatch(typeof(Player))]
        class Player_Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Hit")]
            public static void HitPrefix(Player __instance, HitData hitData)
            {
                //Main.logger.LogInfo("Player Hit " + hitData.damage.damage);
                float newDamage = hitData.damage.damage * Config.playerTakenDamageMult.Value;
                hitData.damage.damage = (int)newDamage;
                //Main.logger.LogInfo("Player Hit mod " + hitData.damage.damage);
            }
            //[HarmonyPostfix]
            //[HarmonyPatch("Update")]
            public static void UpdatePostfix(Player __instance)
            {
                //Test();
            }
        }

        [HarmonyPatch(typeof(PlayerControl), "MoveControls")]
        class PlayerControl_MoveControls_Patch
        {
            public static bool Prefix(PlayerControl __instance)
            {
                MyMoveControls(__instance);
                return false;
            }

            private static void MyMoveControls(PlayerControl __instance)
            { // fux diagonal speed
                if (!__instance.player)
                    __instance.ResetMovement();
                else
                {
                    bool isMoving = false;
                    float z = 0f;
                    float x = 0f;
                    float moveSpeed = __instance.GetMoveSpeed() * Config.playerSpeedMult.Value;
                    float max = 1f;
                    if (__instance.player.status.isAttacking || PlayerEquipment.equippedWeapon && !PlayerEquipment.equippedWeapon.status.attackMomentum && PlayerEquipment.equippedWeapon.status.slowMovement)
                        max = 0.5f;

                    if (PlayerStatus.isExhausted)
                        max *= 0.5f;

                    float f = moveSpeed * Mathf.Clamp(max, 0f, 100f);
                    if (float.IsNaN(f))
                        f = 0f;

                    PlayerControl.sprint = false;
                    if (__instance.CanMove)
                    {
                        bool up = InputManager.KeyHold(InputFunction.Up);
                        bool down = InputManager.KeyHold(InputFunction.Down);
                        bool left = InputManager.KeyHold(InputFunction.Left);
                        bool right = InputManager.KeyHold(InputFunction.Right);
                        if (up)
                            z = 1f;
                        else if (down)
                            z = -PlayerControl.backModifier;
                        
                        if (right)
                            x = PlayerControl.sideModifier;
                        else if (left)
                            x = -PlayerControl.sideModifier;
                        
                        isMoving = up || down || left || right;
                        Vector3 dirVector = new Vector3(x, 0, z).normalized;
                        x = dirVector.x ;
                        z = dirVector.z ;
                        if (up)
                        {
                            z *= f;
                            if (PlayerEquipment.equippedWeapon && PlayerEquipment.equippedWeapon.status.attackMomentum)
                                z += PlayerEquipment.equippedWeapon.attackMomentumBonus;

                            if (__instance.CanSprint && InputManager.KeyHold(InputFunction.Sprint))
                                PlayerControl.sprint = true;
                        }
                        else if (down)
                            z *= f * PlayerControl.backModifier;

                        if (right)
                            x *= f * PlayerControl.sideModifier;
                        else if(left)
                            x *= f * PlayerControl.sideModifier;
                    }
                    if (__instance.attackMomentum > 0)
                    {
                        __instance.attackMomentum -= TimeManager.deltaTime;
                        if (!__instance.charStatus.isAttacking)
                            __instance.attackMomentum = 0f;

                        if (z > 0 && __instance.attackMomentum > 0)
                            z *= PlayerControl.attackMomentumSpeed;
                    }
                    if (PlayerControl.sprint)
                    {
                        z *= PlayerControl.sprintSpeedMod;
                        __instance.player.ConsumeStamina(25f * TimeManager.deltaTime);
                        GameManager.currentGameData.noSprint = false;
                    }
                    __instance.currentMoveVector = new Vector3(Mathf.Lerp(__instance.currentMoveVector.x, x, __instance.moveVectorLerpSpeed * TimeManager.deltaTime), 0f, Mathf.Lerp(__instance.currentMoveVector.z, z, __instance.moveVectorLerpSpeed * TimeManager.deltaTime));
                    if (x == 0f)
                    {
                        if (__instance.currentMoveVector.x > 0f && __instance.currentMoveVector.x <= 0.01f)
                            __instance.currentMoveVector.x = 0f;

                        if (__instance.currentMoveVector.x < 0f && __instance.currentMoveVector.x >= -0.01)
                            __instance.currentMoveVector.x = 0f;
                    }
                    if (z == 0f)
                    {
                        if (__instance.currentMoveVector.z > 0f && __instance.currentMoveVector.z <= 0.01f)
                            __instance.currentMoveVector.z = 0f;

                        if (__instance.currentMoveVector.z < 0f && __instance.currentMoveVector.z >= -0.01f)
                            __instance.currentMoveVector.z = 0f;
                    }
                    Vector3 vector3 = __instance.myTransform.TransformVector(__instance.currentMoveVector);
                    if (__instance.charMove)
                        __instance.charMove.AddMovementVelocity(vector3 * TimeManager.deltaTime);

                    __instance.myTransform.rotation = Quaternion.Euler(0f, __instance.myTransform.rotation.eulerAngles.y, 0f);
                    PlayerControl.isMoving = isMoving;
                    //if (Input.GetKeyDown(KeyCode.LeftControl))
                    //{
                    //    string s = "currentMoveVector magnitude " + PlayerControl.instance.currentMoveVector.magnitude.ToString();
                    //    DungeonEscapeUI.instance.ShowPopupMessage(s, duration: 1, fadeInSpeed: 11f, fadeSpeed: 11f);
                    //}
                }
            }
        }


    }
}
