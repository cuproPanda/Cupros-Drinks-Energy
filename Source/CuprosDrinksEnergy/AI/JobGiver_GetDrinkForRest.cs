using System;

using RimWorld;
using Verse;
using Verse.AI;

namespace CuprosDrinksEnergy {

  public enum DesiredStrength {
    Undefined,
    Weak,
    Medium,
    Strong
  }



  public class JobGiver_GetDrinkForRest : ThinkNode_JobGiver {
    // What strength to start searching for
    public DesiredStrength strength = DesiredStrength.Undefined;


    protected override Job TryGiveJob(Pawn pawn) {

      Thing drink = null;

      // If there is no strength defined, do nothing
      if (strength == 0) {
        return null;
      }

      // If a pawn has caffeine hypersensitivity, they won't want to drink it
      Trait caffTrait = pawn.story.traits.GetTrait(CdeDefOf.CaffeineSensitivity);
      if (caffTrait != null) {
        if (caffTrait.Degree == 2) {
          return null;
        }
      }

      // If a pawn is already wired, they won't drink any more
      // This is to prevent negative effects, as the pawn knows when they've had enough
      Hediff energyDrink = pawn.health.hediffSet.GetFirstHediffOfDef(CdeDefOf.CPD_EnergyDrinkAffector);
      if (energyDrink != null && energyDrink.Severity >= 0.25f) {
        return null;
      }

      // Once a drink is found, make sure it isn't forbidden, and can be reserved
      Predicate<Thing> validator = (Thing t) => (!t.IsForbidden(pawn)) && pawn.CanReserve(t, 1);

      // Find the closest drink, given the following parameters
      if (strength == DesiredStrength.Strong) {
        drink = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(CdeDefOf.CPD_ThrumboEnergy), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.None, TraverseMode.ByPawn, false), 50f, validator);
        // If nothing was found, look for the next drink
        if (drink == null) {
          strength = DesiredStrength.Medium;
        } 
      }

      // Look for dead-eye coffee
      if (strength == DesiredStrength.Medium) {
        drink = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(CdeDefOf.CPD_DECoffee), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.None, TraverseMode.ByPawn, false), 50f, validator);
        // If nothing was found, look for the next drink
        if (drink == null) {
          strength = DesiredStrength.Weak;
        }
      }

      // Look for coffee
      if (strength == DesiredStrength.Weak) {
        drink = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(CdeDefOf.CPD_Coffee), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.None, TraverseMode.ByPawn, false), 50f, validator);
      }

      // If nothing was found, do nothing
      if (drink == null) {
        return null;
      }

      // Otherwise, drink the drink
      return new Job(JobDefOf.Ingest, drink) {
        count = 1
      };
    }
  }
}
