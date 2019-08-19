using SA.Foundation.Editor;


namespace SA.Android
{
    public abstract class AN_APIResolver : SA_iAPIResolver
    {
        //We only using it for ui, when the new build is coming
        //the requirements will be gathered from the build pre process
        private AN_AndroidBuildRequirements m_cachedRequirements = null;
        
        //--------------------------------------
        // Abstract
        //--------------------------------------

        public abstract bool IsSettingsEnabled { get; set; }
        public abstract void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements);

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public void Run(AN_AndroidBuildRequirements buildRequirements) {
            if(IsSettingsEnabled) {
                AppendBuildRequirements(buildRequirements);
            }
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public AN_AndroidBuildRequirements BuildRequirements  {
            get {
                if(m_cachedRequirements == null) {
                
                    m_cachedRequirements = new AN_AndroidBuildRequirements();
                    AppendBuildRequirements(m_cachedRequirements);
                }

                return m_cachedRequirements;
            }
        }


    }
}