using RimWorld;
using Verse;
using Verse.AI;

namespace CuprosDrinksEnergy {

  public class ThinkNode_ConditionalTired : ThinkNode_Conditional {

    // Designates the minimum rest for the selected drink
    public RestCategory minRest;


    protected override bool Satisfied(Pawn pawn) {
      // Try to falsify request prior to scanning
      if (!pawn.IsColonist || pawn.needs.rest == null || minRest == 0) {
        return false;
      }

      // If a pawn has caffeine hypersensitivity, they won't want to drink it
      // Also, teetotalers don't consume chemicals
      if (pawn.story != null) {
        if (pawn.IsTeetotaler()) {
          return false;
        }
        if (pawn.story.traits.DegreeOfTrait(CdeDefOf.CaffeineSensitivity) > 1) {
          return false;
        }
      }

      // Only scan if the pawn is tired enough
      if (pawn.needs.rest.CurCategory >= minRest) {
        // Scan the sleep times, returning true if valid
        if (NotCloseToSleep(pawn)) {
          return true;
        }
      }

      // Otherwise, return false
      return false;
    }


    // Make sure colonists don't drink energy drinks too late
    public bool NotCloseToSleep (Pawn pawn) {
      // Get the current time
      int timeNow = GenLocalDate.HourOfDay(pawn.Map);
      // Whether or not to scan the previous day
      bool scanTom = false;
      // Time to scan for the next day
      int timeTom = 0;

      // Check if the pawn is set to sleep within the next 4 hours
      for (int h = 0; h < 4; h++) {
        // Loop the time to 0 once above 23
        if ((timeNow + h) > 23) {
          scanTom = true;
          timeTom = ((timeNow + h) - 24);
        }
        // If any of the times are sleep times, return false
        if (!scanTom) {
          if (pawn.timetable.times[timeNow + h] == TimeAssignmentDefOf.Sleep) {
            return false;
          }
        }
        if (scanTom) {
          if (pawn.timetable.times[timeTom] == TimeAssignmentDefOf.Sleep) {
            return false;
          }
        }
      }

      return true;
    }
  }
}
