using RimWorld;
using Verse;

namespace CuprosDrinksEnergy {

  public class Hediff_EnergyDrink : HediffWithComps {

    public bool activated;   // Has this hediff been activated?

    // Remove this hediff after it has been activated
    public override bool ShouldRemove {
      get { return activated; }
    }


    public override void PostAdd(DamageInfo? dinfo) {
      base.PostAdd(dinfo);
      // Restore rest equal to current severity (defined in xml)
      EnergizeColonist(Severity);
      // List this as activated so it will be removed
      activated = true;
      return;
    }


    public void EnergizeColonist(float energyToAdd) {
      // Make sure there is valid pawn data
      if (pawn != null && pawn.needs.rest != null && pawn.IsColonist) {

        // Change the energy to add based on pawns sensitivity
        if (pawn.story != null) {
          Trait trait = pawn.story.traits.GetTrait(CdeDefOf.CaffeineSensitivity);
          if (trait != null) {
            if (trait.Degree == 2) {
              energyToAdd *= 2f;
            }
            else if (trait.Degree == 1) {
              energyToAdd *= 1.5f;
            }
            else if (trait.Degree == -1) {
              energyToAdd *= 0.5f;
            }
          }
        }

        // Add energy to the pawn's Rest stat
        pawn.needs.rest.CurLevel += energyToAdd;
      }
    }
  }
}