namespace Taskify_App.Models
{
    /// <summary>
    /// Struct <c>SummaryDetails</c> to hold the summary attributes.
    /// </summary>
    public struct SummaryDetails
    {
        /// <summary>
        /// Project Name
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// Name of the task.
        /// </summary>
        public string taskName { get; set; }

        /// <summary>
        /// Start and stop time of the task.
        /// </summary>
        public DateTime[] timeStamp { get; set; }

        /// <summary>
        /// Total time taken for the task.
        /// </summary>
        public float totalTimeTaken { get; set; }
    }
}
