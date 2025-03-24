namespace Taskify_App.Models
{
    /// <summary>
    /// Class <c>Activity</c> to store the attributes of task.
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// Unique id of the task.
        /// </summary>
        public long TaskID { get; set; }

        //public long TaskID { get; set; }
        /// <summary>
        /// Name of the task.
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Description of the task.
        /// </summary>
        public string TaskDescription { get; set; }

        /// <summary>
        /// List of start and stop time of the timer.
        /// </summary>
        public List<DateTime[]> TimeStamps {  get; set; }

        /// <summary>
        /// Time limit of the task.
        /// </summary>
        public float TimeLimit { get; set; }

        /// <summary>
        /// Total time taken for the task.
        /// </summary>
        public float TotalTimeTaken { get; set; } = 0;

        /// <summary>
        /// Constructor to initialize all the task details.
        /// </summary>
        /// <param name="taskID">ID of the task.</param>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="taskDescription">Description of the task.</param>
        /// <param name="timeStamps">List of stop and start timing.</param>
        /// <param name="timeLimit">Time limit of the task.</param>
        public Activity(long taskID, string taskName, string taskDescription, List<DateTime[]> timeStamps, float timeLimit)
        {
            TaskID = taskID;
            TaskName = taskName;
            TaskDescription = taskDescription;
            TimeStamps = timeStamps;
            TimeLimit = timeLimit;
        }
    }
}
