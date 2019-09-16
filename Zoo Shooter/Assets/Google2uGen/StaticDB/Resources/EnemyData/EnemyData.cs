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
			Rows.Add( new EnemyDataRow("alligator", "normal", "alligator", "alligator", "alligator", "-0.1998554", "-0.2464039", "1.206346", "0.8268769", "0.2493537", "0.5298752", "1.252063", "0.6853374", "1", "0.7", "0.072", "0.092", "kitchenknife"));
			Rows.Add( new EnemyDataRow("alpaca", "normal", "alpaca", "alpaca", "alpaca", "-0.04292327", "-0.3745607", "1.008498", "0.701082", "0.02039872", "0.3941667", "0.9702588", "0.8371118", "1", "0.7", "0.144", "-0.12", "pistol"));
			Rows.Add( new EnemyDataRow("bear", "normal", "bear", "bear", "bear", "-0.02336353", "-0.3467117", "1.047617", "1.043657", "-0.001235187", "0.6502146", "0.987238", "0.9319316", "1", "0.7", "0.245", "-0.004", "fish"));
			Rows.Add( new EnemyDataRow("cat", "normal", "cat", "cat", "cat", "0.07443547", "-0.2689739", "0.8520193", "0.7166571", "0.1608887", "0.3896414", "0.7213029", "0.6243099", "1", "0.8", "0.318", "-0.047", "crow"));
			Rows.Add( new EnemyDataRow("deer", "normal", "deer", "deer", "deer", "-0.03640345", "-0.3056351", "0.8780989", "0.8519724", "0.07101034", "0.5094976", "0.802894", "0.7822218", "1", "0.7", "0.139", "-0.03", "uzi"));
			Rows.Add( new EnemyDataRow("elephant", "normal", "elephant", "elephant", "elephant", "-0.02988353", "-0.2860752", "0.9432982", "0.7085338", "-0.03447647", "0.4521002", "1.052987", "0.7443407", "1", "0.7", "0.217", "-0.03", "pistol"));
			Rows.Add( new EnemyDataRow("giraffe", "normal", "giraffe", "giraffe", "giraffe", "-0.08779693", "-0.439375", "1.140885", "1.096894", "0.1955296", "0.6904638", "0.9331347", "1.133555", "1", "0.7", "0.273", "0.202", "punch"));
			Rows.Add( new EnemyDataRow("Hippo", "normal", "Hippo", "Hippo", "Hippo", "-0.108237", "-0.3062195", "1.100005", "0.6273654", "0.005859554", "0.4098821", "1.05664", "0.7878254", "1", "0.7", "0.234", "-0.021", "pistol"));
			Rows.Add( new EnemyDataRow("octopus", "normal", "octopus", "octopus", "octopus", "-0.002630472", "-0.3337683", "0.9705513", "0.6540275", "-0.08999817", "0.4025195", "0.8559823", "0.7731001", "1", "0.7", "0.032", "0.073", "bubble"));
			Rows.Add( new EnemyDataRow("Penguin", "normal", "Penguin", "Penguin", "Penguin", "-0.02307048", "-0.2549716", "0.9296712", "0.9751413", "0.002098501", "0.569419", "0.7770419", "0.6572191", "1", "0.7", "-0.052", "0.108", "umbrella"));
			Rows.Add( new EnemyDataRow("pig", "normal", "pig", "pig", "pig", "0.01780951", "-0.2792618", "1.011431", "0.7766679", "0.1202748", "0.4827901", "0.8636574", "0.7020956", "1", "0.7", "0.117", "0.073", "claw"));
			Rows.Add( new EnemyDataRow("GasMask", "boss", "GasMask", "GasMask", "GasMask", "-0.1228231", "-0.5644336", "0.8239207", "0.5969703", "-0.02584872", "0.3845963", "0.9157681", "1.259639", "12", "0.7", "0.16", "-0.23", "uzi"));
			Rows.Add( new EnemyDataRow("Clown", "boss", "Clown", "Clown", "Clown", "-0.06082463", "-0.4221901", "1.526323", "1.008424", "0.06620383", "0.6283407", "1.473451", "1.056574", "12", "0.6", "0.16", "-0.02", "repeller"));
			Rows.Add( new EnemyDataRow("SWitch", "boss", "S.Witch", "SWitch", "S.Witch", "-0.03326717", "-0.5353323", "1.177263", "0.8739974", "0.1810261", "0.5237907", "0.9314892", "1.214904", "15", "0.7", "0.14", "-0.23", "broom"));
			Rows.Add( new EnemyDataRow("Surprise", "boss", "Surprise", "Surprise", "Surprise", "-0.03785998", "-0.505514", "1.168077", "1.043864", "-0.03611889", "0.6342686", "1.273921", "1.22322", "15", "0.6", "0.31", "-0.02", "fan"));
			Rows.Add( new EnemyDataRow("Freddy", "boss", "Freddy", "Freddy", "Freddy", "-0.05209363", "-0.4749934", "1.325146", "1.270249", "0.01748228", "0.7309865", "1.278762", "1.131097", "18", "0.6", "0.31", "-0.02", "bread"));
			Rows.Add( new EnemyDataRow("Hotkey", "boss", "Hotkey", "Hotkey", "Hotkey", "-0.005709648", "-0.4946991", "1.232378", "1.120609", "-0.005785435", "0.6539438", "1.084654", "1.144521", "12", "0.6", "0.31", "-0.02", "mic"));
			Rows.Add( new EnemyDataRow("Texas", "boss", "Texas", "Texas", "Texas", "-0.05163854", "-0.4493483", "1.14052", "1.248053", "0.10754", "0.7253698", "0.8028883", "1.085286", "20", "0.7", "0.42", "0.07", "machete"));
			Rows.Add( new EnemyDataRow("Trick", "boss", "Trick", "Trick", "Trick", "-0.0332669", "-0.457956", "1.177263", "1.230838", "0.003720343", "0.6820769", "1.010528", "1.058027", "21", "0.6", "-0.01", "0.07", "fake"));
			Rows.Add( new EnemyDataRow("BadPenguin", "boss", "BadPenguin", "BadPenguin", "BadPenguin", "-0.04704559", "-0.5229973", "1.149706", "1.174243", "-0.05385078", "0.729344", "1.033812", "1.315098", "20", "0.6", "0.17", "-0.07", "umbrella"));
			Rows.Add( new EnemyDataRow("CatAunt", "boss", "CatAunt", "CatAunt", "CatAunt", "0.09840484", "-0.3601084", "1.080444", "0.904366", "0.1233766", "0.5300754", "1.019245", "0.8482307", "23", "0.7", "0.416", "-0.124", "fish"));
			Rows.Add( new EnemyDataRow("Halface", "boss", "Halface", "Halface", "Halface", "-0.02135986", "-0.4439703", "1.121742", "1.083547", "0.1499868", "0.6844109", "0.7843123", "1.156901", "24", "0.7", "0.34", "0.17", "kitchenglove"));
			Rows.Add( new EnemyDataRow("Pigin", "boss", "Pigin", "Pigin", "Pigin", "-0.06678784", "-0.3881848", "1.361272", "1.046445", "0.13713", "0.6424915", "0.9586996", "0.9739468", "26", "0.6", "0.23", "-0.07", "crowbar"));
			Rows.Add( new EnemyDataRow("AilenYo", "boss", "Alien-Yo", "AilenYo", "Alien-Yo", "0.02406824", "-0.3712545", "0.9482901", "1.014228", "0.02975479", "0.5693536", "0.7439492", "0.8276709", "28", "0.6", "0.23", "0.03", "saber"));
			Rows.Add( new EnemyDataRow("PrincessA", "boss", "Princess-A", "PrincessA", "Princess-A", "-0.05852801", "-0.3880848", "1.146521", "0.8318949", "0.1742984", "0.4925079", "0.9174015", "0.9052498", "32", "0.7", "0.371", "0.036", "dryer"));
			Rows.Add( new EnemyDataRow("HeroJe", "boss", "Hero-Je", "HeroJe", "Hero-Je", "-0.05852801", "-0.3038898", "1.146521", "1.000285", "0.343621", "0.5907573", "1.256047", "0.7548437", "30", "0.7", "0.328", "0.135", "saber"));
			Rows.Add( new EnemyDataRow("Father", "boss", "Father", "Father", "Father", "-0.07504725", "-0.5297639", "1.476907", "0.9119605", "0.1784285", "0.5597061", "1.156931", "1.221357", "32", "0.6", "0.433", "-0.14", "saber"));
			Rows.Add( new EnemyDataRow("RHarry", "boss", "R.Harry", "RHarry", "R.Harry", "-0.01309997", "-0.5657891", "1.022627", "1.021621", "0.2959907", "0.5971915", "0.9460348", "1.312607", "25", "0.6", "0.324", "-0.239", "chicken"));
			Rows.Add( new EnemyDataRow("RMarv", "boss", "R.Marv", "RMarv", "R.Marv", "-0.08011448", "-0.3572828", "1.179965", "1.322087", "0.09680909", "0.7830216", "1.076637", "0.9789711", "35", "0.6", "0.181", "0.084", "fowerstrip"));
			Rows.Add( new EnemyDataRow("Gevin", "boss", "Gevin", "Gevin", "Gevin", "-0.01309991", "-0.4219154", "1.045936", "0.9946917", "0.1474674", "0.5803909", "0.6839527", "1.040273", "45", "0.7", "0.194", "0.03", "bubble"));
			Rows.Add( new EnemyDataRow("PinkRabbit", "boss", "PinkRabbit", "PinkRabbit", "PinkRabbit", "0.007295936", "-0.4815128", "1.040109", "1.096936", "0.03729138", "0.6484624", "0.9043048", "1.176416", "46", "0.7", "0.01", "-0.077", "recorder"));
			Rows.Add( new EnemyDataRow("HorrorNurse", "boss", "HorrorNurse", "HorrorNurse", "HorrorNurse", "-0.05971852", "-0.4240343", "1.430542", "1.328441", "0.0658845", "0.7434892", "1.126832", "0.9855505", "48", "0.6", "0.569", "0.18", "crow"));
			Rows.Add( new EnemyDataRow("Triangle", "boss", "Triangle", "Triangle", "Triangle", "0.07709914", "-0.7231239", "1.156906", "0.9848051", "0.1429804", "0.5507553", "1.281024", "1.552082", "40", "0.6", "0.38", "-0.206", "kitchenknife"));
			Rows.Add( new EnemyDataRow("DrBald", "boss", "Dr.Bald", "DrBald", "Dr.Bald", "-0.1323256", "-0.5924091", "1.052896", "1.028377", "-0.1052141", "0.4778156", "0.9640751", "1.101202", "60", "0.8", "0.091", "-0.077", "cheering"));
			Rows.Add( new EnemyDataRow("Greenman", "boss", "Greenman", "Greenman", "Greenman", "-0.2211592", "-0.5391387", "1.085324", "0.6556292", "-0.01756898", "0.5008951", "1.006643", "1.423314", "55", "0.7", "0.339", "-0.191", "frog"));
			Rows.Add( new EnemyDataRow("ChiefBuggle", "boss", "Chief-buggle", "ChiefBuggle", "Chief-buggle", "-0.2533191", "-0.3494413", "1.149644", "1.325502", "-0.1471292", "0.8299384", "0.9317147", "1.009111", "70", "0.7", "0.177", "0.145", "mic"));
			Rows.Add( new EnemyDataRow("Lizard", "boss", "Lizard", "Lizard", "Lizard", "0.1568463", "-0.2790013", "1.118542", "1.223542", "0.2681872", "0.746724", "1.074182", "0.7966335", "80", "0.6", "0.559", "0.182", "banana"));
			Rows.Add( new EnemyDataRow("Sand", "boss", "Sand", "Sand", "Sand", "0.008492112", "-0.3873851", "1.142056", "1.355856", "-0.02023911", "0.7956167", "1.135001", "0.9754644", "75", "0.6", "0.185", "0.041", "bat"));
			Rows.Add( new EnemyDataRow("MagicGirl", "boss", "MagicGirl", "MagicGirl", "MagicGirl", "-0.09395558", "-0.3760019", "1.195177", "1.33309", "-0.09395558", "0.7864844", "1.195177", "1.040329", "75", "0.6", "0.099", "0.019", "nextexit"));
			Rows.Add( new EnemyDataRow("Killer", "boss", "Killer", "Killer", "Killer", "-0.006685063", "-0.3380582", "0.8385068", "1.029541", "0.07295749", "0.6140826", "1.35092", "0.8348997", "80", "0.6", "0.273", "0.126", "watermelon"));
			Rows.Add( new EnemyDataRow("ShooterMan", "boss", "ShooterMan", "ShooterMan", "ShooterMan", "-0.006685078", "-0.3527637", "0.8385068", "1.00013", "0.006002069", "0.5851168", "0.7637931", "0.8680326", "75", "0.6", "0.192", "0.067", "autorifle"));
			Rows.Add( new EnemyDataRow("Diablo", "boss", "Diablo", "Diablo", "Diablo", "0.00849238", "-0.3935344", "0.975104", "1.070363", "0.1501879", "0.6497818", "0.6423735", "0.9973629", "68", "0.6", "0.192", "-0.006", "nofire"));
			Rows.Add( new EnemyDataRow("Hollyshit", "boss", "Hollyshit", "Hollyshit", "Hollyshit", "-0.1660482", "-0.4443003", "1.187588", "0.7867016", "0.05912322", "0.5005181", "0.8245029", "1.063094", "80", "0.6", "0", "-0.006", "bat"));
			Rows.Add( new EnemyDataRow("Pocker", "boss", "Pocker", "Pocker", "Pocker", "0.003230751", "-0.4281788", "1.194497", "1.205868", "0.1732485", "0.7111865", "0.7340269", "1.07015", "80", "0.6", "0.485", "0.11", "bluedragon"));
			Rows.Add( new EnemyDataRow("FishMan", "boss", "FishMan", "FishMan", "FishMan", "0.1839856", "-0.3018242", "1.700412", "0.8809559", "0.170975", "0.5396749", "1.668114", "0.7879046", "85", "0.6", "0.803", "0.044", "kitchenglove"));
			Rows.Add( new EnemyDataRow("StarFish", "boss", "StarFish", "StarFish", "StarFish", "-0.007599413", "-0.4479102", "1.201718", "0.8631535", "0.0107924", "0.5200073", "0.9000928", "1.03738", "68", "0.6", "0.647", "0.511", "fish"));
			Rows.Add( new EnemyDataRow("Sparrow", "boss", "Sparrow", "Sparrow", "Sparrow", "-0.04370067", "-0.411809", "1.129515", "1.079761", "0.03660592", "0.6700568", "0.9639902", "1.085464", "70", "0.6", "0.47", "-0.074", "cluck"));
			Rows.Add( new EnemyDataRow("OctoJohns", "boss", "Octo-Johns", "OctoJohns", "Octo-Johns", "-0.08361039", "-0.4712029", "0.9205238", "1.047617", "-0.07753274", "0.6117294", "0.9323382", "1.1053", "80", "0.6", "0.136", "-0.001", "musket"));
			Rows.Add( new EnemyDataRow("ryu", "boss", "ryu", "ryu", "ryu", "-0.02463999", "-0.3797767", "1.048072", "1.071622", "0.007244825", "0.6223549", "0.921629", "0.924383", "88", "0.7", "0.285", "0.001", "punch"));
			Rows.Add( new EnemyDataRow("ken", "boss", "ken", "ken", "ken", "-0.06685987", "-0.3797767", "1.132512", "1.071622", "-0.03145668", "0.6278977", "0.9990318", "0.9354683", "65", "0.7", "0.285", "0.001", "punch"));
			Rows.Add( new EnemyDataRow("Dhalim", "boss", "Dhalim", "Dhalim", "Dhalim", "-0.09785247", "-0.4250864", "1.151176", "1.16873", "0.03724128", "0.7190736", "0.8616359", "1.11782", "70", "0.7", "0.381", "0.185", "punch"));
			Rows.Add( new EnemyDataRow("Guile", "boss", "Guile", "Guile", "Guile", "-0.1365354", "-0.3622952", "1.07381", "1.034383", "0.09983698", "0.6291846", "1.253644", "0.9380424", "80", "0.6", "0.405", "0.056", "dryer"));
			Rows.Add( new EnemyDataRow("ChunLi", "boss", "Chun-li", "ChunLi", "Chun-li", "-0.1195133", "-0.424541", "1.107854", "0.8625152", "0.08552399", "0.5300767", "0.8126013", "1.071457", "90", "0.6", "0.242", "-0.095", "macaron"));
			Rows.Add( new EnemyDataRow("HeyGrandfa", "boss", "HeyGrandfa", "HeyGrandfa", "HeyGrandfa", "-0.001073748", "-0.3872137", "0.8235996", "0.9371696", "-0.01317555", "0.5608439", "0.7731214", "0.9522012", "80", "0.6", "0.173", "0.05", "sniper"));
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
