using UnityEngine;
using TheNextFlow.UnityPlugins;

// Script must be place in Editor folder
[LocalizedAppNameAPI]
public class APIDemo {
	static void APIDemoExample1() {
		new LocalizedAppNameData(Language.English, "Hello world")
			.putLocalization(Language.Spanish, "Hola mundo")
			.Apply();
	}
	
	static void APIDemoExample2() {
		new LocalizedAppNameData(Language.French, "Bonjour monde")
			.putLocalization(Language.Italian, "Ciao mondo")
			.putLocalization(Language.German, "Hallo welt")
			.Apply();
	}
}