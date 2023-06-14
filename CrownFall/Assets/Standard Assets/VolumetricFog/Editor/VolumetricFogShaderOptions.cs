using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;

namespace VolumetricFogAndMist {
	
	public class VolumetricFogShaderOptions {

		public bool pendingChanges;
		public ShaderAdvancedOption[] options;

		public void ReadOptions() {
			pendingChanges = false;
			// Populate known options
			options = new ShaderAdvancedOption[]
			{
				new ShaderAdvancedOption
				{
					id = "FOG_ORTHO", name = "Orthographic Mode", description = "Enables support for orthographic camera projection."
				},
				new ShaderAdvancedOption
				{
					id = "FOG_DEBUG",
					name = "Debug Mode",
					description = "Enables fog debug view."
				},
				new ShaderAdvancedOption
				{
					id = "FOG_MASK",
					name = "Enable Mask",
					description = "Enables mask defined by geometry volumes (meshes)."
				}
			};


			Shader shader = Shader.Find("VolumetricFogAndMist/VolumetricFog");
			if (shader != null) {
				string path = AssetDatabase.GetAssetPath(shader);
				string file = Path.GetDirectoryName(path) + "/VolumetricFogOptions.cginc";
				string[] lines = File.ReadAllLines(file, Encoding.UTF8);
				for (int k = 0; k < lines.Length; k++) {
					for (int o = 0; o < options.Length; o++) {
						if (lines[k].Contains(options[o].id)) {
							options[o].enabled = !lines[k].StartsWith("//");
						}
					}
				}
			}
		}


		public bool GetAdvancedOptionState(string optionId) {
			if (options == null)
				return false;
			for (int k = 0; k < options.Length; k++) {
				if (options[k].id.Equals(optionId)) {
					return options[k].enabled;
				}
			}
			return false;
		}

		public void UpdateAdvancedOptionsFile() {
			// Reloads the file and updates it accordingly
			Shader shader = Shader.Find("VolumetricFogAndMist/VolumetricFog");
			if (shader != null) {
				string path = AssetDatabase.GetAssetPath(shader);
				string file = Path.GetDirectoryName(path) + "/VolumetricFogOptions.cginc";
				string[] lines = File.ReadAllLines(file, Encoding.UTF8);
				for (int k = 0; k < lines.Length; k++) {
					for (int o = 0; o < options.Length; o++) {
						if (lines[k].Contains(options[o].id)) {
							if (options[o].enabled) {
								lines[k] = "#define " + options[o].id;
							} else {
								lines[k] = "//#define " + options[o].id;
							}
						}
					}
				}
				File.WriteAllLines(file, lines, Encoding.UTF8);
				pendingChanges = false;
				AssetDatabase.Refresh();
			}
		}


	}

	public struct ShaderAdvancedOption {
		public string id;
		public string name;
		public string description;
		public bool enabled;
	}


}