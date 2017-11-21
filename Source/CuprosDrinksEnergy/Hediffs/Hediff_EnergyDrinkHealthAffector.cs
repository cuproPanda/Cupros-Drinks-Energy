using RimWorld;
using Verse;

namespace CuprosDrinksEnergy {

  public class Hediff_EnergyDrinkHealthAffector : HediffWithComps {


    public override void PostAdd(DamageInfo? dinfo) {
      base.PostAdd(dinfo);

      // Offset the severity based on pawn sensitivity
      Severity += GetOffsetForPawn();
    }


    private float GetOffsetForPawn() {
      if (pawn != null && pawn.story != null) {
        Trait trait = pawn.story.traits.GetTrait(CdeDefOf.CaffeineSensitivity);
        if (trait != null) {
          if (trait.Degree == 2) {
            return def.initialSeverity;
          }
          if (trait.Degree == 1) {
            return (def.initialSeverity * 0.5f);
          }
          if (trait.Degree == -1) {
            return -(def.initialSeverity * 0.5f);
          }
        }
      }
      return 0;
    }
  }
}
