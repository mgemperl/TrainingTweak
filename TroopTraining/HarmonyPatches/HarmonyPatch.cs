using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace TrainingTweak.HarmonyPatches;

public abstract class HarmonyPatch
{
   private readonly Harmony _harmony;
   private bool _applied;

   protected HarmonyPatch(Harmony harmony)
   {
      _harmony = harmony;
   }

   public void Apply()
   {
      if (_applied) return;
      
      try
      {
         PatchMethodWithPostfix(OriginalMethod, Postfix);
         _applied= true;
      }
      catch (Exception) { }
   }

   [CanBeNull]
   protected abstract MethodInfo OriginalMethod { get; }
   protected abstract MethodInfo Postfix { get; }

   private void PatchMethodWithPostfix([CanBeNull] MethodInfo original, MethodInfo postfix)
   {
      if (original is null)
         throw new InvalidOperationException(
            $"Original method not found for patch: {postfix.Name}");
      
      _harmony.Patch(
         original: original,
         postfix: new HarmonyMethod(postfix));
   }

   private void UnpatchMethod([CanBeNull] MethodInfo original, MethodInfo patch)
   {
      _harmony.Unpatch(original, patch);
   }
}