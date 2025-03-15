using Taskify_App.Enums;

namespace Taskify_App.Models
{
    public class Activity
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public List<DateTime[]> TimeStamps {  get; set; }
        public int TimeLimit { get; set; }
        public int TotalTimeTaken { get; set; } = 0;

        public Activity(string taskName, string taskDescription, List<DateTime[]> timeStamps, int timeLimit)
        {
            TaskName = taskName;
            TaskDescription = taskDescription;
            TimeStamps = timeStamps;
            TimeLimit = timeLimit;
        }
    }
}
