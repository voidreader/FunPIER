using SA.Foundation.Editor;

namespace SA.Android
{
    public abstract class AN_APIResolver : SA_iAPIResolver
    {
        //We only using it for ui, when the new build is coming
        //the requirements will be gathered from the build pre process
        private AN_AndroidBuildRequirements m_CachedRequirements;
        
        //--------------------------------------
        // Abstract
        //--------------------------------------

        public abstract bool IsSettingsEnabled { get; set; }
        public void ResetRequirementsCache()
        {
            m_CachedRequirements = null;
        }

        protected abstract void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements);

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public void Run(AN_AndroidBuildRequirements buildRequirements) 
        {
            if(IsSettingsEnabled) 
                AppendBuildRequirements(buildRequirements);
        }
        
        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public AN_AndroidBuildRequirements BuildRequirements 
        {
            get 
            {
                if(m_CachedRequirements == null) 
                {
                    m_CachedRequirements = new AN_AndroidBuildRequirements();
                    AppendBuildRequirements(m_CachedRequirements);
                }

                return m_CachedRequirements;
            }
        }
    }
}