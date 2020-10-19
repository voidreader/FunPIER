using SA.Foundation.Editor;

namespace SA.Android.Editor
{
    /// <summary>
    /// Base class to implement Android Native Pro plugin post process directives.
    /// </summary>
    public abstract class AN_APIResolver : SA_iAPIResolver
    {
        //We only using it for ui, when the new build is coming
        //the requirements will be gathered from the build pre process
        AN_AndroidBuildRequirements m_CachedRequirements;

        /// <summary>
        /// Returns <c>true</c> if attached settings is enabled.
        /// </summary>
        public abstract bool IsSettingsEnabled { get; set; }

        /// <summary>
        /// Reset Resolver requirements cache.
        /// </summary>
        public void ResetRequirementsCache()
        {
            m_CachedRequirements = null;
        }

        /// <summary>
        /// Append build resolver requirements.
        /// </summary>
        /// <param name="buildRequirements">Build Requirements.</param>
        protected abstract void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements);

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        /// <summary>
        /// Run resolver.
        /// </summary>
        /// <param name="buildRequirements">Build Requirements.</param>
        public void Run(AN_AndroidBuildRequirements buildRequirements)
        {
            if (IsSettingsEnabled)
                AppendBuildRequirements(buildRequirements);
        }

        //--------------------------------------
        // Get / Set
        //--------------------------------------

        /// <summary>
        /// Current Resolver Build Requirements.
        /// </summary>
        public AN_AndroidBuildRequirements BuildRequirements
        {
            get
            {
                if (m_CachedRequirements == null)
                {
                    m_CachedRequirements = new AN_AndroidBuildRequirements();
                    AppendBuildRequirements(m_CachedRequirements);
                }

                return m_CachedRequirements;
            }
        }
    }
}
