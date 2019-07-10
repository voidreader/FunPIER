//----------------------------------------------
//    Google2u: Google Doc Unity integration
//         Copyright © 2015 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;
using System.Globalization;

namespace Google2u
{
	[System.Serializable]
	public class EnemyDataRow : IGoogle2uRow
	{
		public string _enemytype;
		public string _sprite;
		public string _identifier;
		public string _name;
		public float _offsetX;
		public float _offsetY;
		public float _sizeX;
		public float _sizeY;
		public float _hoffsetX;
		public float _hoffsetY;
		public float _hsizeX;
		public float _hsizeY;
		public int _hp;
		public float _scale;
		public float _gunposX;
		public float _gunposY;
		public string _gun;
		public EnemyDataRow(string __ID, string __enemytype, string __sprite, string __identifier, string __name, string __offsetX, string __offsetY, string __sizeX, string __sizeY, string __hoffsetX, string __hoffsetY, string __hsizeX, string __hsizeY, string __hp, string __scale, string __gunposX, string __gunposY, string __gun) 
		{
			_enemytype = __enemytype.Trim();
			_sprite = __sprite.Trim();
			_identifier = __identifier.Trim();
			_name = __name.Trim();
			{
			float res;
				if(float.TryParse(__offsetX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_offsetX = res;
				else
					Debug.LogError("Failed To Convert _offsetX string: "+ __offsetX +" to float");
			}
			{
			float res;
				if(float.TryParse(__offsetY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_offsetY = res;
				else
					Debug.LogError("Failed To Convert _offsetY string: "+ __offsetY +" to float");
			}
			{
			float res;
				if(float.TryParse(__sizeX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_sizeX = res;
				else
					Debug.LogError("Failed To Convert _sizeX string: "+ __sizeX +" to float");
			}
			{
			float res;
				if(float.TryParse(__sizeY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_sizeY = res;
				else
					Debug.LogError("Failed To Convert _sizeY string: "+ __sizeY +" to float");
			}
			{
			float res;
				if(float.TryParse(__hoffsetX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hoffsetX = res;
				else
					Debug.LogError("Failed To Convert _hoffsetX string: "+ __hoffsetX +" to float");
			}
			{
			float res;
				if(float.TryParse(__hoffsetY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hoffsetY = res;
				else
					Debug.LogError("Failed To Convert _hoffsetY string: "+ __hoffsetY +" to float");
			}
			{
			float res;
				if(float.TryParse(__hsizeX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hsizeX = res;
				else
					Debug.LogError("Failed To Convert _hsizeX string: "+ __hsizeX +" to float");
			}
			{
			float res;
				if(float.TryParse(__hsizeY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hsizeY = res;
				else
					Debug.LogError("Failed To Convert _hsizeY string: "+ __hsizeY +" to float");
			}
			{
			int res;
				if(int.TryParse(__hp, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_hp = res;
				else
					Debug.LogError("Failed To Convert _hp string: "+ __hp +" to int");
			}
			{
			float res;
				if(float.TryParse(__scale, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_scale = res;
				else
					Debug.LogError("Failed To Convert _scale string: "+ __scale +" to float");
			}
			{
			float res;
				if(float.TryParse(__gunposX, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_gunposX = res;
				else
					Debug.LogError("Failed To Convert _gunposX string: "+ __gunposX +" to float");
			}
			{
			float res;
				if(float.TryParse(__gunposY, NumberStyles.Any, CultureInfo.InvariantCulture, out res))
					_gunposY = res;
				else
					Debug.LogError("Failed To Convert _gunposY string: "+ __gunposY +" to float");
			}
			_gun = __gun.Trim();
		}

		public int Length { get { return 17; } }

		public string this[int i]
		{
		    get
		    {
		        return GetStringDataByIndex(i);
		    }
		}

		public string GetStringDataByIndex( int index )
		{
			string ret = System.String.Empty;
			switch( index )
			{
				case 0:
					ret = _enemytype.ToString();
					break;
				case 1:
					ret = _sprite.ToString();
					break;
				case 2:
					ret = _identifier.ToString();
					break;
				case 3:
					ret = _name.ToString();
					break;
				case 4:
					ret = _offsetX.ToString();
					break;
				case 5:
					ret = _offsetY.ToString();
					break;
				case 6:
					ret = _sizeX.ToString();
					break;
				case 7:
					ret = _sizeY.ToString();
					break;
				case 8:
					ret = _hoffsetX.ToString();
					break;
				case 9:
					ret = _hoffsetY.ToString();
					break;
				case 10:
					ret = _hsizeX.ToString();
					break;
				case 11:
					ret = _hsizeY.ToString();
					break;
				case 12:
					ret = _hp.ToString();
					break;
				case 13:
					ret = _scale.ToString();
					break;
				case 14:
					ret = _gunposX.ToString();
					break;
				case 15:
					ret = _gunposY.ToString();
					break;
				case 16:
					ret = _gun.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "enemytype":
					ret = _enemytype.ToString();
					break;
				case "sprite":
					ret = _sprite.ToString();
					break;
				case "identifier":
					ret = _identifier.ToString();
					break;
				case "name":
					ret = _name.ToString();
					break;
				case "offsetX":
					ret = _offsetX.ToString();
					break;
				case "offsetY":
					ret = _offsetY.ToString();
					break;
				case "sizeX":
					ret = _sizeX.ToString();
					break;
				case "sizeY":
					ret = _sizeY.ToString();
					break;
				case "hoffsetX":
					ret = _hoffsetX.ToString();
					break;
				case "hoffsetY":
					ret = _hoffsetY.ToString();
					break;
				case "hsizeX":
					ret = _hsizeX.ToString();
					break;
				case "hsizeY":
					ret = _hsizeY.ToString();
					break;
				case "hp":
					ret = _hp.ToString();
					break;
				case "scale":
					ret = _scale.ToString();
					break;
				case "gunposX":
					ret = _gunposX.ToString();
					break;
				case "gunposY":
					ret = _gunposY.ToString();
					break;
				case "gun":
					ret = _gun.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "enemytype" + " : " + _enemytype.ToString() + "} ";
			ret += "{" + "sprite" + " : " + _sprite.ToString() + "} ";
			ret += "{" + "identifier" + " : " + _identifier.ToString() + "} ";
			ret += "{" + "name" + " : " + _name.ToString() + "} ";
			ret += "{" + "offsetX" + " : " + _offsetX.ToString() + "} ";
			ret += "{" + "offsetY" + " : " + _offsetY.ToString() + "} ";
			ret += "{" + "sizeX" + " : " + _sizeX.ToString() + "} ";
			ret += "{" + "sizeY" + " : " + _sizeY.ToString() + "} ";
			ret += "{" + "hoffsetX" + " : " + _hoffsetX.ToString() + "} ";
			ret += "{" + "hoffsetY" + " : " + _hoffsetY.ToString() + "} ";
			ret += "{" + "hsizeX" + " : " + _hsizeX.ToString() + "} ";
			ret += "{" + "hsizeY" + " : " + _hsizeY.ToString() + "} ";
			ret += "{" + "hp" + " : " + _hp.ToString() + "} ";
			ret += "{" + "scale" + " : " + _scale.ToString() + "} ";
			ret += "{" + "gunposX" + " : " + _gunposX.ToString() + "} ";
			ret += "{" + "gunposY" + " : " + _gunposY.ToString() + "} ";
			ret += "{" + "gun" + " : " + _gun.ToString() + "} ";
			return ret;
		}
	}
	public sealed class EnemyData : IGoogle2uDB
	{
		public enum rowIds {
			alligator, alpaca, bear, cat, deer, elephant, giraffe, Hippo, octopus, Penguin, pig, GasMask, Clown, SWitch, Surprise, Freddy, Hotkey, Texas
			, Trick, BadPenguin, CatAunt, Halface, Pigin, AilenYo, PrincessA, HeroJe, Father, RHarry, RMarv, Gevin, PinkRabbit, HorrorNurse, Triangle, DrBald, Greenman, ChiefBuggle, Lizard, Sand
			, MagicGirl, Killer, ShooterMan, Diablo, Hollyshit, Pocker, FishMan, StarFish, Sparrow, OctoJohns, ryu, ken, Dhalim, Guile, ChunLi, HeyGrandfa
		};
		public string [] rowNames = {
			"alligator", "alpaca", "bear", "cat", "deer", "elephant", "giraffe", "Hippo", "octopus", "Penguin", "pig", "GasMask", "Clown", "SWitch", "Surprise", "Freddy", "Hotkey", "Texas"
			, "Trick", "BadPenguin", "CatAunt", "Halface", "Pigin", "AilenYo", "PrincessA", "HeroJe", "Father", "RHarry", "RMarv", "Gevin", "PinkRabbit", "HorrorNurse", "Triangle", "DrBald", "Greenman", "ChiefBuggle", "Lizard", "Sand"
			, "MagicGirl", "Killer", "ShooterMan", "Diablo", "Hollyshit", "Pocker", "FishMan", "StarFish", "Sparrow", "OctoJohns", "ryu", "ken", "Dhalim", "Guile", "ChunLi", "HeyGrandfa"
		};
		public System.Collections.Generic.List<EnemyDataRow> Rows = new System.Collections.Generic.List<EnemyDataRow>();

		public static EnemyData Instance
		{
			get { return NestedEnemyData.instance; }
		}

		private class NestedEnemyData
		{
			static NestedEnemyData() { }
			internal static readonly EnemyData instance = new EnemyData();
		}

		private EnemyData()
		{
			Rows.Add( new EnemyDataRow("alligator", "normal", "alligator", "alligator", "alligator", "-0.1998554", "-0.258684", "1.206346", "0.8023167", "0.2868674", "0.4918259", "1.095168", "0.5437449", "1", "0.7", "0.072", "0.092", "kitchenknife"));
			Rows.Add( new EnemyDataRow("alpaca", "normal", "alpaca", "alpaca", "alpaca", "-0.04292327", "-0.3871344", "1.008498", "0.6759346", "0.06168884", "0.3097668", "0.7693776", "0.668312", "1", "0.7", "0.144", "-0.12", "pistol"));
			Rows.Add( new EnemyDataRow("bear", "normal", "bear", "bear", "bear", "-0.02336353", "-0.3643147", "1.047617", "1.008451", "0.1301482", "0.5281846", "0.6324589", "0.6878716", "1", "0.7", "0.245", "-0.004", "fish"));
			Rows.Add( new EnemyDataRow("cat", "normal", "cat", "cat", "cat", "0.07443547", "-0.2795554", "0.8520193", "0.6954941", "0.2638068", "0.3488864", "0.4694605", "0.4987931", "1", "0.8", "0.318", "-0.047", "crow"));
			Rows.Add( new EnemyDataRow("deer", "normal", "deer", "deer", "deer", "-0.03640345", "-0.3056351", "0.8780989", "0.8519724", "0.2344671", "0.3749661", "0.4759805", "0.4466337", "1", "0.7", "0.139", "-0.03", "uzi"));
			Rows.Add( new EnemyDataRow("elephant", "normal", "elephant", "elephant", "elephant", "-0.02988353", "-0.2860752", "0.9432982", "0.7085338", "0.1203682", "0.384746", "0.7432979", "0.6096321", "1", "0.7", "0.217", "-0.03", "pistol"));
			Rows.Add( new EnemyDataRow("giraffe", "normal", "giraffe", "giraffe", "giraffe", "-0.08779693", "-0.439375", "1.140885", "1.096894", "0.2181673", "0.5999039", "0.5607398", "0.9225887", "1", "0.7", "0.273", "0.202", "punch"));
			Rows.Add( new EnemyDataRow("Hippo", "normal", "Hippo", "Hippo", "Hippo", "-0.108237", "-0.3133284", "1.100005", "0.6131476", "0.1466274", "0.3444041", "0.6765664", "0.6568694", "1", "0.7", "0.234", "-0.021", "pistol"));
			Rows.Add( new EnemyDataRow("octopus", "normal", "octopus", "octopus", "octopus", "-0.002630472", "-0.3337683", "0.9705513", "0.6540275", "-0.006672591", "0.3512175", "0.6016197", "0.6704962", "1", "0.7", "0.032", "0.073", "bubble"));
			Rows.Add( new EnemyDataRow("Penguin", "normal", "Penguin", "Penguin", "Penguin", "-0.02307048", "-0.2656351", "0.9296712", "0.9538144", "-0.006672591", "0.5249574", "0.6016197", "0.568296", "1", "0.7", "-0.052", "0.108", "umbrella"));
			Rows.Add( new EnemyDataRow("pig", "normal", "pig", "pig", "pig", "0.01780951", "-0.2792618", "1.011431", "0.7766679", "0.1772873", "0.4329778", "0.6970063", "0.5751091", "1", "0.7", "0.117", "0.073", "claw"));
			Rows.Add( new EnemyDataRow("GasMask", "boss", "GasMask", "GasMask", "GasMask", "-0.1228231", "-0.5644336", "0.8239207", "0.5969703", "-0.02584872", "0.2454677", "0.9157681", "0.9813813", "12", "0.7", "0.16", "-0.23", "uzi"));
			Rows.Add( new EnemyDataRow("Clown", "boss", "Clown", "Clown", "Clown", "-0.06082463", "-0.4320275", "1.526323", "0.9887496", "0.06620383", "0.5348855", "1.473451", "0.8696633", "12", "0.6", "0.16", "-0.02", "repeller"));
			Rows.Add( new EnemyDataRow("SWitch", "boss", "S.Witch", "SWitch", "S.Witch", "-0.03326717", "-0.5606285", "1.177263", "0.8234051", "0.1810261", "0.2455335", "0.9314892", "0.6583898", "15", "0.7", "0.14", "-0.23", "broom"));
			Rows.Add( new EnemyDataRow("Surprise", "boss", "Surprise", "Surprise", "Surprise", "-0.03785998", "-0.505514", "1.168077", "1.043864", "0.2407336", "0.5900006", "0.7202165", "1.016635", "15", "0.6", "0.31", "-0.02", "fan"));
			Rows.Add( new EnemyDataRow("Freddy", "boss", "Freddy", "Freddy", "Freddy", "-0.005709648", "-0.54685", "1.232378", "1.126536", "0.1672473", "0.5900006", "0.7385881", "1.016635", "18", "0.6", "0.31", "-0.02", "bread"));
			Rows.Add( new EnemyDataRow("Hotkey", "boss", "Hotkey", "Hotkey", "Hotkey", "-0.005709648", "-0.5192927", "1.232378", "1.071422", "0.1672473", "0.5900006", "0.7385881", "1.016635", "12", "0.6", "0.31", "-0.02", "mic"));
			Rows.Add( new EnemyDataRow("Texas", "boss", "Texas", "Texas", "Texas", "-0.05163854", "-0.4825494", "1.14052", "1.181651", "0.10754", "0.6405224", "0.8028883", "0.9155917", "20", "0.8", "0.42", "0.07", "machete"));
			Rows.Add( new EnemyDataRow("Trick", "boss", "Trick", "Trick", "Trick", "-0.0332669", "-0.4825494", "1.177263", "1.181651", "0.10754", "0.6083721", "0.8028883", "0.8512912", "21", "0.6", "-0.01", "0.07", "fake"));
			Rows.Add( new EnemyDataRow("BadPenguin", "boss", "BadPenguin", "BadPenguin", "BadPenguin", "-0.04704559", "-0.5376641", "1.149706", "1.144909", "0.0845755", "0.4751779", "0.7569594", "0.7869906", "20", "0.6", "0.17", "-0.07", "umbrella"));
			Rows.Add( new EnemyDataRow("CatAunt", "boss", "CatAunt", "CatAunt", "CatAunt", "0.09840484", "-0.3887506", "1.080444", "0.8470815", "0.3394909", "0.432692", "0.5870159", "0.6534637", "23", "0.7", "0.34", "-0.124", "fish"));
			Rows.Add( new EnemyDataRow("Halface", "boss", "Halface", "Halface", "Halface", "-0.02135986", "-0.4754768", "1.121742", "1.020534", "0.294063", "0.6185338", "0.49616", "1.025147", "24", "0.7", "0.34", "0.17", "kitchenglove"));
			Rows.Add( new EnemyDataRow("Pigin", "boss", "Pigin", "Pigin", "Pigin", "-0.06678784", "-0.4383085", "1.361272", "0.9461974", "0.13713", "0.5689761", "0.9586996", "0.8269161", "26", "0.6", "0.23", "-0.07", "crowbar"));
			Rows.Add( new EnemyDataRow("AilenYo", "boss", "Alien-Yo", "AilenYo", "Alien-Yo", "0.02406824", "-0.3846209", "0.9482901", "0.9874956", "0.02975479", "0.5359377", "0.7439492", "0.760839", "28", "0.6", "0.23", "0.03", "saber"));
			Rows.Add( new EnemyDataRow("PrincessA", "boss", "Princess-A", "PrincessA", "Princess-A", "-0.05852801", "-0.4052701", "1.146521", "0.7975242", "0.1742984", "0.4409517", "0.9174015", "0.8021374", "32", "0.7", "0.286", "0.036", "dryer"));
			Rows.Add( new EnemyDataRow("HeroJe", "boss", "Hero-Je", "HeroJe", "Hero-Je", "-0.05852801", "-0.3268035", "1.146521", "0.9544572", "0.343621", "0.4905095", "1.256047", "0.5543482", "30", "0.7", "0.286", "0.279", "saber"));
			Rows.Add( new EnemyDataRow("Father", "boss", "Father", "Father", "Father", "-0.07504725", "-0.5498135", "1.476907", "0.8718611", "0.1784285", "0.5028991", "1.156931", "1.107743", "32", "0.6", "0.433", "-0.14", "saber"));
			Rows.Add( new EnemyDataRow("RHarry", "boss", "R.Harry", "RHarry", "R.Harry", "-0.01309997", "-0.5787222", "1.022627", "0.9957552", "0.4096982", "0.2468504", "1.17345", "0.5130491", "25", "0.6", "0.373", "-0.14", "chicken"));
			Rows.Add( new EnemyDataRow("RMarv", "boss", "R.Marv", "RMarv", "R.Marv", "-0.08011448", "-0.3572828", "1.179965", "1.322087", "0.2698418", "0.7396539", "0.7305714", "0.7294455", "35", "0.6", "0.181", "0.084", "fowerstrip"));
			Rows.Add( new EnemyDataRow("Gevin", "boss", "Gevin", "Gevin", "Gevin", "-0.01309991", "-0.4388656", "1.045936", "0.9607912", "0.1474674", "0.4861642", "0.6839527", "0.8518198", "45", "0.7", "0.126", "0.03", "bubble"));
			Rows.Add( new EnemyDataRow("PinkRabbit", "boss", "PinkRabbit", "PinkRabbit", "PinkRabbit", "0.007295936", "-0.4942254", "1.040109", "1.071511", "0.1474674", "0.4045813", "0.6839527", "0.688654", "46", "0.7", "0.01", "-0.077", "recorder"));
			Rows.Add( new EnemyDataRow("HorrorNurse", "boss", "HorrorNurse", "HorrorNurse", "HorrorNurse", "-0.05971852", "-0.4388655", "1.430542", "1.298778", "0.0658845", "0.7046899", "1.126832", "0.7527548", "48", "0.6", "0.471", "0.249", "crow"));
			Rows.Add( new EnemyDataRow("Triangle", "boss", "Triangle", "Triangle", "Triangle", "0.07709914", "-0.7379552", "1.156906", "0.9551425", "0.1429804", "0.4278907", "1.281024", "1.306353", "40", "0.6", "0.34", "-0.087", "kitchenknife"));
			Rows.Add( new EnemyDataRow("DrBald", "boss", "Dr.Bald", "DrBald", "Dr.Bald", "-0.006413102", "-0.6072403", "0.8010712", "0.9987143", "-0.009520501", "0.3080686", "0.772688", "0.761708", "60", "0.8", "0.091", "-0.077", "cheering"));
			Rows.Add( new EnemyDataRow("Greenman", "boss", "Greenman", "Greenman", "Greenman", "-0.04272282", "-0.5564067", "0.7284517", "0.6210933", "0.1320873", "0.4787244", "0.7073306", "1.378973", "55", "0.7", "0.339", "-0.191", "frog"));
			Rows.Add( new EnemyDataRow("ChiefBuggle", "boss", "Chief-buggle", "ChiefBuggle", "Chief-buggle", "-0.2533191", "-0.3494413", "1.149644", "1.325502", "0.0485751", "0.758309", "0.5403061", "0.8198035", "70", "0.7", "0.177", "0.145", "mic"));
			Rows.Add( new EnemyDataRow("Lizard", "boss", "Lizard", "Lizard", "Lizard", "0.2930693", "-0.2887314", "0.8460957", "1.204082", "0.4044101", "0.6820586", "0.8017362", "0.6673026", "80", "0.6", "0.53", "0.182", "banana"));
			Rows.Add( new EnemyDataRow("Sand", "boss", "Sand", "Sand", "Sand", "0.008492112", "-0.3873851", "1.142056", "1.355856", "0.1729539", "0.6972362", "0.7486153", "0.7583672", "75", "0.6", "0.185", "0.041", "bat"));
			Rows.Add( new EnemyDataRow("MagicGirl", "boss", "MagicGirl", "MagicGirl", "MagicGirl", "-0.09395558", "-0.3760019", "1.195177", "1.33309", "0.214692", "0.7541516", "0.5892519", "0.8721981", "75", "0.6", "0.099", "0.019", "nextexit"));
			Rows.Add( new EnemyDataRow("Killer", "boss", "Killer", "Killer", "Killer", "-0.006685063", "-0.3380582", "0.8385068", "1.029541", "0.3550835", "0.4847516", "1.173584", "0.5762379", "80", "0.6", "0.192", "0.15", "watermelon"));
			Rows.Add( new EnemyDataRow("ShooterMan", "boss", "ShooterMan", "ShooterMan", "ShooterMan", "-0.006685078", "-0.3608243", "0.8385068", "0.9840086", "0.006002069", "0.5075178", "0.7637931", "0.7128348", "75", "0.6", "0.192", "0.067", "autorifle"));
			Rows.Add( new EnemyDataRow("Diablo", "boss", "Diablo", "Diablo", "Diablo", "0.00849238", "-0.3987679", "0.975104", "1.059896", "0.1501879", "0.5075178", "0.6423735", "0.7128348", "68", "0.6", "0.192", "-0.006", "nofire"));
			Rows.Add( new EnemyDataRow("Hollyshit", "boss", "Hollyshit", "Hollyshit", "Hollyshit", "-0.1660482", "-0.4443003", "1.187588", "0.7867016", "0.05912322", "0.4164531", "0.8245029", "0.8949643", "80", "0.6", "0", "-0.006", "bat"));
			Rows.Add( new EnemyDataRow("Pocker", "boss", "Pocker", "Pocker", "Pocker", "0.003230751", "-0.4443002", "1.194497", "1.173625", "0.1732485", "0.5947882", "0.7340269", "0.8114879", "80", "0.6", "0.207", "-0.006", "bluedragon"));
			Rows.Add( new EnemyDataRow("FishMan", "boss", "FishMan", "FishMan", "FishMan", "0.2162286", "-0.3179455", "1.764898", "0.8487132", "0.2274003", "0.428722", "1.780965", "0.5659987", "85", "0.6", "0.488", "0.722", "kitchenglove"));
			Rows.Add( new EnemyDataRow("StarFish", "boss", "StarFish", "StarFish", "StarFish", "-0.007599413", "-0.4479102", "1.201718", "0.8631535", "0.0107924", "0.4359423", "0.9000928", "0.86925", "68", "0.6", "0.21", "4.27", "fish"));
			Rows.Add( new EnemyDataRow("Sparrow", "boss", "Sparrow", "Sparrow", "Sparrow", "-0.04370067", "-0.411809", "1.129515", "1.079761", "0.2418409", "0.5153652", "0.5535202", "0.7104042", "70", "0.6", "0.388", "0", "cluck"));
			Rows.Add( new EnemyDataRow("OctoJohns", "boss", "Octo-Johns", "OctoJohns", "Octo-Johns", "-0.02203983", "-0.4876217", "0.7973828", "1.014779", "-0.003648043", "0.3854005", "0.7845688", "0.6526417", "80", "0.6", "0.136", "-0.001", "musket"));
			Rows.Add( new EnemyDataRow("ryu", "boss", "ryu", "ryu", "ryu", "0.01406156", "-0.3973683", "0.9706694", "1.036439", "0.1479774", "0.5225855", "0.6401638", "0.7248442", "88", "0.7", "0.285", "0.001", "punch"));
			Rows.Add( new EnemyDataRow("ken", "boss", "ken", "ken", "ken", "0.01406156", "-0.3973683", "0.9706694", "1.036439", "0.1479774", "0.5225855", "0.6401638", "0.7248442", "65", "0.7", "0.285", "0.001", "punch"));
			Rows.Add( new EnemyDataRow("Dhalim", "boss", "Dhalim", "Dhalim", "Dhalim", "-0.0978525", "-0.4443001", "1.151176", "1.130303", "0.2165698", "0.6417199", "0.5029789", "0.963113", "70", "0.7", "0.317", "0.255", "punch"));
			Rows.Add( new EnemyDataRow("Guile", "boss", "Guile", "Guile", "Guile", "-0.1195132", "-0.3793176", "1.107854", "1.000338", "0.3934665", "0.428722", "1.160023", "0.5371171", "80", "0.6", "0.362", "0.056", "dryer"));
			Rows.Add( new EnemyDataRow("ChunLi", "boss", "Chun-li", "ChunLi", "Chun-li", "-0.1195133", "-0.4543293", "1.107854", "0.8029385", "0.08552399", "0.3813462", "0.8126013", "0.7739962", "90", "0.6", "0.242", "-0.095", "macaron"));
			Rows.Add( new EnemyDataRow("HeyGrandfa", "boss", "HeyGrandfa", "HeyGrandfa", "HeyGrandfa", "-0.001073748", "-0.3872137", "0.8235996", "0.9371696", "-0.01317555", "0.5155778", "0.7731214", "0.7582049", "80", "0.6", "0.173", "0.05", "sniper"));
		}
		public IGoogle2uRow GetGenRow(string in_RowString)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}
		public IGoogle2uRow GetGenRow(rowIds in_RowID)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public EnemyDataRow GetRow(rowIds in_RowID)
		{
			EnemyDataRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public EnemyDataRow GetRow(string in_RowString)
		{
			EnemyDataRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}

	}

}
