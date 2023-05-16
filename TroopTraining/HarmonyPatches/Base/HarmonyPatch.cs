using System;
using System.Reflection;
using HarmonyLib;

namespace TrainingTweak.HarmonyPatches.Base;

public abstract class HarmonyPatch
{
   protected Harmony Harmony { get; }
   private bool _applied;
   
   protected abstract MethodInfo? OriginalMethod { get; }
   protected abstract MethodInfo? Patch { get; }

   protected HarmonyPatch(Harmony harmony)
   {
      Harmony = harmony;
   }
   
   public void Apply()
   {
      if (_applied || OriginalMethod is null || Patch is null) 
         return;
      
      try
      {
         PatchMethod(OriginalMethod, Patch);
         _applied= true;
      }
      catch (Exception) { }
   }

   protected abstract void PatchMethod(
      MethodInfo original, MethodInfo patch);
   
   protected void UnpatchMethod(MethodInfo original, MethodInfo patch)
   {
      Harmony.Unpatch(original, patch);
   }
}