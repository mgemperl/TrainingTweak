using System;
using System.Reflection;
using HarmonyLib;

namespace TrainingTweak.HarmonyPatches.Base;

public abstract class HarmonyPrefix : HarmonyPatch
{
   protected HarmonyPrefix(Harmony harmony) : base(harmony) { }
   protected override void PatchMethod(MethodInfo original, MethodInfo prefix)
   {
      if (original is null)
         throw new InvalidOperationException(
            $"Original method not found for patch: {prefix.Name}");
      
      Harmony.Patch(
         original: original,
         prefix: new HarmonyMethod(prefix));
   }
}