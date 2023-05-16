using System;
using System.Reflection;
using HarmonyLib;

namespace TrainingTweak.HarmonyPatches.Base;

public abstract class HarmonyPostfix : HarmonyPatch
{
   protected HarmonyPostfix(Harmony harmony) : base(harmony) {}

   protected override void PatchMethod(MethodInfo original, MethodInfo postfix)
   {
      if (original is null)
         throw new InvalidOperationException(
            $"Original method not found for patch: {postfix.Name}");
      
      Harmony.Patch(
         original: original,
         postfix: new HarmonyMethod(postfix));
   }
}