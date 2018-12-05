using System;
using System.Collections.Generic;
using System.Linq;
using FPSService.Logging;
using FPSService.SQL;
using Microsoft.SqlServer.Types;

namespace FPSService.BeatData
{
    /*
    public class BeatClass
    {
        public Guid BeatID { get; set; }
        public string BeatDescription { get; set; }
        public string FreewayName { get; set; }
        public bool Active { get; set; }
        public SqlGeography BeatExtent { get; set; }
        public Guid BeatSegmentID { get; set; }
        public string BeatSegmentDescription { get; set; }
        public SqlGeography BeatSegmentExtent { get; set; }
        public string CHPDescription { get; set; }
        public string PIMSID { get; set; }
    }
     * */

    public class Beat
    {
        public Guid BeatID { get; set; }
        public string BeatDescription { get; set; }
        public SqlGeography BeatExtent { get; set; }
        public int FreewayID { get; set; }
        public string BeatNumber { get; set; }
        public bool IsTemporary { get; set; }
    }

    public class BeatSegment
    {
        public Guid BeatSegmentID { get; set; }
        public string CHPDescription { get; set; }
        public SqlGeography BeatSegmentExtent { get; set; }
        public string BeatSegmentNumber { get; set; }
        public string BeatSegmentDescription { get; set; }
        public Guid BeatID { get; set; }
    }

    public static class Beats
    {
        //public static List<BeatClass> AllBeats = new List<BeatClass>();
        public static List<Beat> AllBeats = new List<Beat>();
        public static List<BeatSegment> AllBeatSegments = new List<BeatSegment>();
        private static readonly EventLogger logger = new EventLogger();

        public static void LoadBeats() //completely reinitializes in-memory beat list
        {
            try
            {
                var mySQL = new SQLCode();
                AllBeats = mySQL.LoadBeatsOnly();
            }
            catch (Exception ex)
            {
                logger.LogEvent(
                    DateTime.Now + Environment.NewLine + "Error Loading All Beats" + Environment.NewLine + ex, true);
            }
        }

        public static void LoadBeatSegments()
        {
            try
            {
                var mySQL = new SQLCode();
                AllBeatSegments = mySQL.LoadSegmentsOnly();
            }
            catch (Exception ex)
            {
                logger.LogEvent(
                    DateTime.Now + Environment.NewLine + "Error Loading All Beat Segments" + Environment.NewLine + ex,
                    true);
            }
        }

        public static void AddBeat(Beat thisBeat)
        {
            //only add distinct beats
            try
            {
                var newBeat = AllBeats.Find(delegate(Beat b) { return b.BeatID == thisBeat.BeatID; });
                if (newBeat == null) AllBeats.Add(thisBeat);
            }
            catch (Exception ex)
            {
                logger.LogEvent(DateTime.Now + Environment.NewLine + "Error Adding Beat" + Environment.NewLine + ex,
                    true);
            }
        }

        public static Beat GetDriverBeat(Guid BeatID)
        {
            try
            {
                var thisBeat = AllBeats.Find(b => b.BeatID == BeatID);
                if (thisBeat != null)
                    return thisBeat;
                return null;
            }
            catch (Exception ex)
            {
                logger.LogEvent(
                    DateTime.Now + Environment.NewLine + "Error Finding Driver Beat By ID" + Environment.NewLine + ex,
                    true);
                return null;
            }
        }

        public static List<Beat> FindBeatByID(Guid BeatID) //Given a beat id, locate all elements and segments
        {
            try
            {
                var thisBeatData = new List<Beat>();
                var lstBeat = from ab in AllBeats
                    where ab.BeatID == BeatID
                    select ab;
                foreach (var thisBeat in lstBeat) thisBeatData.Add(thisBeat);
                if (thisBeatData.Count > 0)
                    return thisBeatData;
                return null;
            }
            catch (Exception ex)
            {
                logger.LogEvent(
                    DateTime.Now + Environment.NewLine + "Error Finding Beat By ID" + Environment.NewLine + ex, true);
                return null;
            }
        }

        public static string FindBeatSegmentNameByID(Guid _BeatSegmentID)
        {
            var thisFoundSegment = AllBeatSegments.Find(delegate(BeatSegment bs)
            {
                return bs.BeatSegmentID == _BeatSegmentID;
            });
            if (thisFoundSegment != null)
                return thisFoundSegment.BeatSegmentNumber;
            return "Unknown";
        }

        public static string FindBeat(SqlGeography thisGeography)
        {
            var BeatDescription = "No Beat";
            var bList = from ab in AllBeats
                select ab;
            foreach (var thisBeat in bList)
                if (thisBeat.BeatExtent.STIntersects(thisGeography))
                    //if (thisBeat.BeatExtent.STContains(thisGeography))
                    BeatDescription = thisBeat.BeatDescription;
            return BeatDescription;
        }

        public static bool FindOnBeat(Guid BeatID, SqlGeography thisGeography)
        {
            var OnBeat = false;
            var bList = from ab in AllBeats
                where ab.BeatID == BeatID
                select ab;
            foreach (var thisBeat in bList)
                if (thisBeat.BeatExtent.STBuffer(10).STIntersects(thisGeography))
                    OnBeat = true;
            return OnBeat;
        }

        public static string FindCurrentBeatSegment(Guid BeatID, SqlGeography thisGeography)
        {
            var currentSegment = "NA";
            var bList = from ab in AllBeatSegments
                where ab.BeatID == BeatID
                select ab;
            foreach (var thisBeatSegment in bList)
                if (thisBeatSegment.BeatSegmentExtent.STBuffer(10).STIntersects(thisGeography))
                    currentSegment = thisBeatSegment.BeatSegmentDescription;
            return currentSegment;
        }

        public static Guid FindBeatIDByName(string BeatName)
        {
            var BeatID = new Guid("00000000-0000-0000-0000-000000000000");

            var bList = from b in AllBeats
                where b.BeatNumber == BeatName
                select b;

            foreach (var b in bList) BeatID = b.BeatID;

            return BeatID;
        }

        public static Guid FindCurrentBeatSegmentID(Guid BeatID, SqlGeography thisGeography)
        {
            var segmentID = new Guid("00000000-0000-0000-0000-000000000000");

            var bList = from ab in AllBeatSegments
                where ab.BeatID == BeatID
                select ab;
            foreach (var bs in bList)
                if (bs.BeatSegmentExtent.STBuffer(10).STIntersects(thisGeography))
                    segmentID = bs.BeatSegmentID;

            return segmentID;
        }

        public static Guid FindBeatID(SqlGeography thisGeography)
        {
            var BeatID = new Guid("00000000-0000-0000-0000-000000000000");
            var bList = from ab in AllBeats
                select ab;
            foreach (var thisBeat in bList)
                if (thisBeat.BeatExtent.STBuffer(10).STIntersects(thisGeography))
                {
                    BeatID = thisBeat.BeatID;
                    return BeatID;
                }

            return BeatID;
        }

        public static Guid FindBeatSegmentIDbyName(Guid BeatID, string segmentName)
        {
            var segID = new Guid("00000000-0000-0000-0000-000000000000");
            var bList = from abs in AllBeatSegments
                where abs.BeatID == BeatID && abs.BeatSegmentDescription == segmentName
                select abs;
            foreach (var bs in bList) segID = bs.BeatSegmentID;
            return segID;
        }

        public static Guid FindBeatSegmentID(Guid BeatID, SqlGeography thisGeography)
        {
            var BeatSegmentID = new Guid("00000000-0000-0000-0000-000000000000");
            var bList = from abs in AllBeatSegments
                where abs.BeatID == BeatID
                select abs;
            foreach (var thisBeatSegment in bList)
                if (thisBeatSegment.BeatSegmentExtent.STBuffer(10).STIntersects(thisGeography))
                {
                    BeatSegmentID = thisBeatSegment.BeatSegmentID;
                    return BeatSegmentID;
                }

            return BeatSegmentID;
        }

        public static Beat FindBeatSegment(string BeatDescription, SqlGeography thisGeography)
        {
            var foundBeat = new Beat();
            var bList = from ab in AllBeats
                where ab.BeatDescription == BeatDescription
                select ab;
            foreach (var thisBeat in bList)
                if (thisBeat.BeatExtent.STBuffer(10).STIntersects(thisGeography))
                    foundBeat = thisBeat;
            return foundBeat;
        }

        public static List<Beat> FindBeatByDescription(string Description)
        {
            var thisBeatData = new List<Beat>();
            var lstBeat = from ab in AllBeats
                where ab.BeatDescription == Description
                select ab;
            foreach (var thisBeat in lstBeat) thisBeatData.Add(thisBeat);
            if (thisBeatData.Count > 0)
                return thisBeatData;
            return null;
        }

        public static List<Beat> FindBeatsByFreeway(int FreewayID)
        {
            var FreewayBeats = new List<Beat>();
            foreach (var b in AllBeats)
                if (b.FreewayID == FreewayID)
                    FreewayBeats.Add(b);
            if (FreewayBeats.Count > 0)
                return FreewayBeats;
            return null;
        }

        /*
        public static List<Beat> FindSegmentsByBeat(string BeatDescription)
        {
            List<Beat> BeatSegments = new List<Beat>();
            var BeatList = from ab in AllBeats
                           where ab.BeatDescription == BeatDescription
                           select ab;
            foreach (BeatClass thisBeat in BeatList)
            {
                BeatSegments.Add(thisBeat);
            }
            if (BeatSegments.Count > 0)
            {
                return BeatSegments;
            }
            else
            {
                return null;
            }
        }

        public static SqlGeography FindBeatSegmentByID(Guid _BeatSegmentID)
        {
            SqlGeography thisGeo = null;
            var BeatSegList = from abs in AllBeats
                              where abs.BeatSegmentID == _BeatSegmentID
                              select abs;
            foreach (BeatClass thisBeat in BeatSegList)
            {
                thisGeo = thisBeat.BeatSegmentExtent;
            }
            return thisGeo;
        }
         * */
    }
}