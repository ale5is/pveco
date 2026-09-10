using TMPro;
using UnityEngine;

namespace StartScene
{
	public class AlmcPlantLoader : MonoBehaviour
	{
		public static AlmcPlantLoader Instance;

		public SpriteRenderer MapImg;

		public TextMesh NameText;

		public TextMeshPro MainText;

		public TextMeshPro ValueText;

		public TextMeshPro CdText;

		private PlantBase DisplayPlant;

		private ZombieBase DisplayZombie;

		private ZombieType CurrZType;

		public Sprite PlantCard;

		public Sprite ZombieCard;

		public Sprite NormalMap;

		public Sprite NightMap;

		public Sprite DayWaterMap;

		public Sprite NightWaterMap;

		public Sprite RoofMap;

		public Sprite SwampMap;

		private void Awake()
		{
			Instance = this;
		}

		public void LoadPlant(PlantType plantType, ZombieType zombieType, string sunNum, float cd)
		{
			if ((plantType == PlantType.Nope && zombieType == ZombieType.Nope) || (DisplayPlant != null && DisplayPlant.GetPlantType() == plantType))
			{
				return;
			}
			if (DisplayPlant != null)
			{
				Object.Destroy(DisplayPlant.gameObject);
			}
			if (DisplayZombie != null && CurrZType == zombieType)
			{
				return;
			}
			if (DisplayZombie != null)
			{
				Object.Destroy(DisplayZombie.gameObject);
			}
			if (plantType != PlantType.Nope)
			{
				DisplayPlant = PlantManager.Instance.GetNewPlant(plantType);
				DisplayPlant.transform.SetParent(base.transform.parent);
				DisplayPlant.InitForAlmanac(new Vector2(-30.5f, -48f));
			}
			else
			{
				CurrZType = zombieType;
				DisplayZombie = ZombieManager.Instance.GetNewZombie(zombieType);
				DisplayZombie.transform.SetParent(base.transform.parent);
				DisplayZombie.InitForAlmanac(new Vector2(-30.5f, -48f));
			}
			string text = "非常长";
			if (cd < 8f)
			{
				text = "短";
			}
			else if (cd < 25f)
			{
				text = "中等";
			}
			else if (cd < 40f)
			{
				text = "长";
			}
			LoadBackground(0);
			if (plantType != PlantType.Nope)
			{
				NameText.text = PlantManager.Instance.GetPlantName(plantType);
				base.transform.GetComponent<SpriteRenderer>().sprite = PlantCard;
			}
			else
			{
				NameText.text = ZombieManager.Instance.GetZombieName(zombieType);
				base.transform.GetComponent<SpriteRenderer>().sprite = ZombieCard;
			}
			ValueText.text = "花费:<color=#D54130>" + sunNum + "</color>";
			CdText.text = "恢复时间:<color=#D54130>" + text + "</color>";
			switch (plantType)
			{
			case PlantType.SunFlower:
				MainText.text = "<color=#2B345B>向日葵，为你生产更多阳光的基础作物。尽可能多地种植吧！</color>\n特性：<color=#52BE00>在夜晚产量降低。</color>\n阳光产量：<color=#D54130>中等</color>\n向日葵情不自禁地和着节拍起舞。是什么节拍呢？嗨，是大地自己用来提神的爵士节拍，这种频率的节拍，只有向日葵才能听到。";
				break;
			case PlantType.PeaShooter:
				MainText.text = "<color=#2B345B>豌豆射手，你的第一道防线，发射豌豆攻击僵尸。</color>\n伤害：<color=#D54130>中等</color>\n一棵植物，怎么能如此快地生长，并发射如此多的豌豆呢？豌豆射手说：“努力工作，奉献自己，加上一份阳光，高纤维和二氧化碳均衡搭配，这种健康早餐让一切皆成可能。”";
				break;
			case PlantType.WallNut:
				MainText.text = "<color=#2B345B>坚果拥有足以保护其它植物的坚硬外壳。</color>\n强度：<color=#D54130>高</color>\n坚果说：“人们想知道，经常被僵尸啃的感觉怎样？他们不知道，我有限的感官，只能让我感到一种麻麻的感觉，就像是令人放松的背部按摩。”";
				break;
			case PlantType.Cherry:
				MainText.text = "<color=#2B345B>樱桃炸弹，可以炸掉一定范围内的全部僵尸。他们只有很短的引线，一种下就会立刻引爆，所以请把他们种在僵尸身边。</color>\n伤害：<color=#D54130>巨大</color>\n范围：<color=#D54130>中等范围内的全部僵尸</color>\n用法：<color=#D54130>一次性使用，立即爆炸</color>\n“我要‘爆’开了，”樱桃一号说。“不，我们是要‘炸’开了！”它弟弟樱桃二号说。经过激烈的商议之后，他们才统一“爆炸”这个说法。";
				break;
			case PlantType.Tallnut:
				LoadBackground(2);
				MainText.text = "<color=#2B345B>高坚果是重型壁垒植物，它不会被僵尸从头上跳过。</color>\n强度：<color=#D54130>非常高</color>\n特点：<color=#D54130>不会被跨过或跳过</color>\n人们想知道，坚果和高坚果是否在竞争。高坚果以男中音的声调大声笑了。“我们之间怎么会存在竞争关系？我们是兄弟。你知道坚果为我做了什么吗……”高坚果的声音越来越小，他狡黠地笑着。";
				break;
			case PlantType.Lilypad:
				LoadBackground(4);
				MainText.text = "<color=#2B345B>莲叶可以让你在它上面种植非水生植物。</color>\n特点：<color=#D54130>可以在上面种植非水生植物</color>\n<color=#21A4D4>必须种植在水面</color>\n莲叶从不抱怨，它也从来不想知道发生了什么事。在它身上种植物，它也不会说什么。难道，它有什么惊奇想法或者可怕的秘密？ 没人知道。莲叶把这些都埋藏在心底。";
				break;
			case PlantType.Spike:
				LoadBackground(2);
				MainText.text = "<color=#2B345B>地刺可以扎破轮胎，并对踩到他的僵尸造成伤害。</color>\n伤害：<color=#D54130>普通</color>\n范围：<color=#D54130>所有踩到它的僵尸</color>\n特点：<color=#D54130>不会被僵尸吃掉</color>\n地刺痴迷冰球运动，他买了包厢的季票。他一直关注着他喜欢的球员，他也始终如一的在赛后清理冰球场。但有一个问题：他害怕冰球。";
				break;
			case PlantType.Repeater:
				MainText.text = "<color=#2B345B>双重射手可以一次发射两颗豌豆。</color>\n伤害：<color=#D54130>中等（每颗）</color>\n发射速度：<color=#D54130>两倍</color>\n双重射手很凶悍，他是在街头混大的。他不在乎任何人的看法，无论是植物还是僵尸，他打出豌豆，是为了能够让别人离他远一点。其实呢，双重射手一直暗暗地渴望着爱情。";
				break;
			case PlantType.Torchwood:
				LoadBackground(2);
				MainText.text = "<color=#2B345B>火炬树桩可以把穿过他的豌豆变成火球，让豌豆造成两倍伤害。</color>\n特点：<color=#D54130>让穿过他的火球造成两倍伤害。火球也会对附近僵尸造成溅射伤害。</color>\n每个人都喜欢并敬重火炬树桩。他们喜欢他的诚实和坚贞的友谊，以及增强豌豆伤害的能力。但他也有自己的秘密：他不识字！";
				break;
			case PlantType.Jalapeno:
				LoadBackground(2);
				MainText.text = "<color=#2B345B>火爆辣椒可以摧毁一整条线上的僵尸。</color>\n伤害：<color=#D54130>极高</color>\n范围：<color=#D54130>整条线上的僵尸</color>\n用法：<color=#D54130>一次性使用，立即生效</color>\n“嘎嘎嘎嘎嘎嘎嘎！！！”火爆辣椒说。他现在不会爆炸，因为还不到时候，不过快了，喔~，快了快了，快到了。是的，他感受到了，他一生都在等待这个时刻！";
				break;
			case PlantType.SnowPea:
				MainText.text = "<color=#2B345B>寒冰射手会发射寒冰豌豆攻击敌人，并具有减速效果。</color>\n伤害：<color=#D54130>中等，具有低级减速效果</color>\n人们经常告诉寒冰射手他是多么“冷酷”或者告诫他要“冷静”。他们叫他要“保持镇静”。寒冰射手只是转转他的眼睛。其实他都听见了。”";
				break;
			case PlantType.PotatoMine:
				MainText.text = "<color=#2B345B>土豆地雷具有强大的威力，但是他们需要点时间来启动自己。你应把他们种在僵尸前进的路上，当他们一被碰到就会爆炸。</color>\n伤害：<color=#D54130>巨大</color>\n范围：<color=#D54130>小范围内全部的僵尸</color>\n用法：<color=#D54130>一次性使用，需要一定时间准备才能启动。</color>\n一些人说土豆雷很懒，因为他总是把所有事情留到最后。土豆雷才没空理他们，他正忙着考虑他的投资战略呢。”";
				break;
			case PlantType.Squash:
				LoadBackground(2);
				MainText.text = "<color=#2B345B>窝瓜会压扁第一个接近它的僵尸。</color>\n伤害：<color=#D54130>极高</color>\n范围：<color=#D54130>压扁接近它的僵尸</color>\n用法：<color=#D54130>一次性使用</color>\n“我准备好了！”窝瓜大吼道，“干吧！！算我一份！没人比我厉害！我就是你要的人！来啊！等啥啊？要的就是这个！”";
				break;
			case PlantType.Chomper:
				MainText.text = "<color=#2B345B>大嘴花可以一口吞掉一整只僵尸，但在他们咀嚼僵尸的时候很脆弱。</color>\n特性：<color=#52BE00>咀嚼时间根据僵尸强度变化</color>\n范围：<color=#D54130>非常近</color>\n大嘴花差一点就要去“恐怖小店”表演它的绝技了，不过他的经济人榨取了他太多的钱，所以他没去成。尽管如此，大嘴花没有怨言，只说了句：“这只是交易的一部分。”";
				break;
			case PlantType.Tanglekelp:
				LoadBackground(4);
				MainText.text = "<color=#2B345B>缠绕水草是一种可以把接近他的僵尸拉进水中的水生植物。</color>\n伤害：<color=#D54130>极高</color>\n用法：<color=#D54130>一次性使用，接触后生效</color>\n<color=#21A4D4>必须种植在水面</color>\n“我是完全隐形的，”缠绕水草自己想，“我就藏在水面下，没人会看到我。”他的朋友告诉他，他们可以清楚地看到他。不过缠绕水草似乎不想改变自己的看法。";
				break;
			case PlantType.ThreePeater:
				LoadBackground(2);
				MainText.text = "<color=#2B345B>三重射手可以在三条线上同时射出豌豆。</color>\n伤害：<color=#D54130>中等（每颗豌豆的伤害力）</color>\n范围：<color=#D54130>三线</color>\n三重射手喜欢读书，下棋以及在公园里呆坐。他也喜欢演出，特别是现代爵士乐。“我正在寻找我生命中的另一半。”他说。三重射手最爱的数字是5。";
				break;
			case PlantType.PuffShroom:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>小喷菇是免费的，不过射程很近。</color>\n伤害：<color=#D54130>中等</color>\n范围：<color=#D54130>近</color>\n<color=#8E39A8>白天要睡觉</color>\n小喷菇说：“我也是最近才知道僵尸的存在的，和很多蘑菇一样，我只是把他们想象成童话和电影里的怪物。现在的经历才让我大开了眼界。”";
				break;
			case PlantType.SunShroom:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>阳光菇开始提供少量阳光，稍后提供较大数量阳光。</color>\n特性：<color=#52BE00>在白天产量大幅降低。</color>\n阳光产量：<color=#D54130>开始低，之后正常</color>\n<color=#8E39A8>白天要睡觉</color>\n阳光菇讨厌阳光。恨到当它内部产生点阳光时，就尽可能快的吐出来。它就是不能忍受这个。对它来说，阳光令人厌恶。";
				break;
			case PlantType.FumeShroom:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>大喷菇喷出的气泡可以穿透铁丝网门。</color>\n伤害：<color=#D54130>普通，可以穿透铁丝网门</color>\n范围：<color=#D54130>被气泡喷中的僵尸</color>\n<color=#8E39A8>白天要睡觉</color>\n“我以前那份没前途的工作，是为一个面包房生产酵母孢，”大喷菇说。“然后小喷菇，上帝保佑它，告诉了我这个喷杀僵尸的机会。现在我真觉得自己完全不同了。”";
				break;
			case PlantType.Gravebuster:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>墓碑吞噬者会吃掉墓碑。</color>\n用法：<color=#D54130>单次使用，只对墓碑有效</color>\n尽管咬咬碑的外表十分吓人，但他想要所有人都知道，其实他喜欢小猫咪。而且他利用业余时间在一家僵尸康复中心做志愿者。“我只是在做正确的事情，”他说。";
				break;
			case PlantType.HypnoShroom:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>当僵尸吃下魅惑菇后，他将会掉转方向为你作战。</color>\n用法：<color=#D54130>一次性使用，接触生效</color>\n<color=#8E39A8>白天要睡觉</color>\n魅惑菇这样说道：“僵尸是我们的朋友，他们被严重误解了，僵尸们在我们的生态环境里扮演着重要角色。我们可以，也应当更努力地让他们学会用我们的方式来思考。”";
				break;
			case PlantType.ScaredyShroom:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>胆小菇是一种远程射手，敌人接近后会躲起来。</color>\n伤害：<color=#D54130>中等</color>\n特点：<color=#D54130>敌人接近后停止攻击</color>\n<color=#8E39A8>白天要睡觉</color>\n“谁在那？”胆小菇低声说，声音细微难辨。“走开！我不想见任何人。除非……除非你是马戏团的人。”";
				break;
			case PlantType.IceShroom:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>寒冰菇能短暂的冻结屏幕上所有僵尸。</color>\n伤害：<color=#D54130>非常低，冻结僵尸</color>\n范围：<color=#D54130>当前场地上的所有僵尸</color>\n<color=#8E39A8>白天要睡觉</color>\n冰川菇皱着眉头，倒不是因为它不高兴或不满意，只是因为，它儿时因受创伤而遗留下了面瘫。";
				break;
			case PlantType.DoomShroom:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>毁灭菇可以摧毁大范围的僵尸，并留下一个不能种植物的大弹坑。</color>\n伤害：<color=#D54130>极高</color>\n范围：<color=#D54130>大范围内的所有僵尸</color>\n特点：<color=#D54130>留下一个弹坑</color>\n<color=#8E39A8>白天要睡觉</color>\n“你很幸运，我是和你一伙的，”末日菇说，“我能摧毁任何你所珍视的东西，小菜一碟。”";
				break;
			case PlantType.Blover:
				LoadBackground(3);
				MainText.text = "<color=#2B345B>三叶草，能吹走所有的气球僵尸，也可以把雾吹散。</color>\n特性：<color=#52BE00>微微将僵尸吹退。</color>\n用法：<color=#D54130>一次性使用，立即生效</color>\n特点：<color=#D54130>吹走屏幕上所有的气球僵尸</color>\n当三叶草五岁生日的时候，他得到了一个闪亮的生日蛋糕。他许好愿，然后深吸一口气却只吹灭了60 % 的蜡烛。然而他没有放弃，小时候的那次失败促使他更加努力直到现在。";
				break;
			case PlantType.SeaShroom:
				LoadBackground(5);
				MainText.text = "<color=#2B345B>水兵菇，能够发射短程孢子的水生植物。</color>\n伤害：<color=#D54130>普通</color>\n范围：<color=#D54130>近</color>\n<color=#21A4D4>必须种植在水面</color>\n<color=#8E39A8>白天要睡觉</color>\n水兵菇从来没看到过大海，虽然他的名字叫水兵，他总听到关于大海的事。他只是没找到合适的时间，总有一天……是的，他会见到海的。";
				break;
			case PlantType.Pot:
				LoadBackground(6);
				MainText.text = "<color=#2B345B>花盆可以让你在屋顶上种植植物。</color>\n特点：<color=#D54130>可以让你在屋顶种植</color>\n“我是一个让植物栽种的花盆，但我也是一棵植物。是不是很意外？”";
				break;
			case PlantType.Plantern:
				LoadBackground(3);
				MainText.text = "<color=#2B345B>路灯花，能照亮一片区域，为你驱散战场迷雾。</color>\n范围：<color=#D54130>一片圆形区域</color>\n特点：<color=#D54130>驱散战场迷雾</color>\n特性：<color=#52BE00>可增幅向日葵类的产量。</color>\n路灯花拒绝科学，他只会埋头苦干。其他植物吃的是光，挤出的是氧气。路灯花吃的是黑暗，挤出的却是光。对于他如何能产生光这件事，路灯花持谨慎态度。“我不会说这是‘巫术’，我也不会使用‘原力的黑暗面’，我只是……我想我说得够多的了。”";
				break;
			case PlantType.GatlingPea:
				MainText.text = "<color=#2B345B>机枪射手一次可以发射六颗豌豆。</color>\n伤害：<color=#D54130>中等（每颗豌豆）</color>\n发射速度：<color=#D54130>六倍</color>\n当机枪射手宣布他要参军的时候，他的父母很为他担心，他们异口同声地对他说：“亲爱的，这太危险了。”机枪射手拒绝让步，“生活本就危险”他这样回答，此时他的眼睛里，正闪烁着钢铁般的信念。";
				break;
			case PlantType.TwinSunflower:
				MainText.text = "<color=#2B345B>双胞向日葵提供两倍的阳光产量。</color>\n特性：<color=#52BE00>在夜晚产量降低。</color>\n阳光产量：<color=#D54130>双倍</color>\n这是一个疯狂的夜晚，禁忌的科学技术，让双胞向日葵来到这个世界。电闪雷鸣，狂风怒吼，都在表示着这个世界对他的拒绝。但是一切都无济于事，双胞向日葵仍然活着！";
				break;
			case PlantType.SpikeRock:
				MainText.text = "<color=#2B345B>钢地刺可以扎破多个轮胎，并对踩到他的僵尸造成伤害。</color>\n钢地刺刚刚从欧洲旅行回来。他玩的很高兴，也认识了很多有趣的人。这些都真的拓展了他的视野——他从来不知道，他们建造了这么大的博物馆，有这么多的画作。这对他来说太惊奇了。";
				break;
			case PlantType.Marigold:
				MainText.text = "<color=#2B345B>金盏花可以提供金币和银币。</color>\n特点：<color=#D54130>提供金币和银币</color>\n金盏花花了许多时间，来考虑是吐出一枚金币，还是吐出一枚银币。她权衡着各个角度，不停地思索着。她做了相当充足的研究，并且，时刻注意最新的出版资料。这就是成功者们总是最先进的原因。";
				break;
			case PlantType.SplitPea:
				LoadBackground(3);
				MainText.text = "<color=#2B345B>双向射手可以向前后两个方向发射豌豆。</color>\n伤害：<color=#D54130>中等</color>\n范围：<color=#D54130>前面和后面</color>\n速度：<color=#D54130>向前为正常速度，向后为两倍速度</color>\n双向射手：“没错，我就是双子座。我知道，这的确很令人惊奇。不过有两个头，或者实际上，长着一个头和一个类似头的东西，在背上，对我这条线上的防守帮助很大。”";
				break;
			case PlantType.Cabbagepult:
				MainText.text = "<color=#2B345B>卷心菜投手可以投掷卷心菜。</color>\n伤害：<color=#D54130>中等</color>\n范围：<color=#D54130>三行抛掷</color>\n卷心菜投手用小卷心菜漂亮地砸僵尸。他以此赚钱，毕竟，他擅长这个。只是首先他不明白僵尸们是怎么爬上屋顶的。";
				break;
			case PlantType.Cornpult:
				MainText.text = "<color=#2B345B>玉米投手，可以投掷玉米粒和黄油。</color>\n伤害：<color=#D54130>轻微（玉米粒），中等（黄油）</color>\n范围：<color=#D54130>抛掷</color>\n特点：<color=#D54130>黄油可以暂时固定住僵尸</color>\n玉米投手是兄弟中的老大。他们三兄弟当中，只有玉米投手记得其他人的生日，只是他记得不太准。";
				break;
			case PlantType.Melonpult:
				MainText.text = "<color=#2B345B>西瓜投手能对成群的僵尸造成巨大伤害。</color>\n伤害：<color=#D54130>大</color>\n范围：<color=#D54130>抛掷</color>\n特点：<color=#D54130>西瓜能对目标附近的僵尸造成伤害</color>\n低调从来不是西瓜投手的风格，“太阳——赐予我——力量，我可是草地上最能打的人”，他说“我不是吹牛，瞅瞅那些统计数字，你就就明白了。”";
				break;
			case PlantType.Wintermelonpult:
				MainText.text = "<color=#2B345B>冰西瓜能造成范围性的伤害并让僵尸减速。</color>\n伤害：<color=#D54130>大</color>\n范围：<color=#D54130>投掷</color>\n特点：<color=#D54130>击中后造成范围伤害并减速</color>\n冰西瓜试着让自己紧绷的神经冷静下来。他听到僵尸的接近。他能做到么？有人能做到么？";
				break;
			case PlantType.GloomShroom:
				LoadBackground(1);
				MainText.text = "<color=#2B345B>忧郁菇围绕自身释放大量烟雾。</color>\n“我一直喜欢释放大量烟雾，”忧郁菇说。“我知道很多人受不了这样，他们说这又粗鲁又恶臭。我只能说，你们更想被僵尸吃掉大脑么？”";
				break;
			case PlantType.Magnetshroom:
				LoadBackground(3);
				MainText.text = "<color=#2B345B>磁力菇可以用磁力吸取僵尸的头盔等其它金属物品。</color>\n范围：<color=#D54130>靠近的僵尸</color>\n特点：<color=#D54130>移除僵尸们所有的金属物品</color>\n<color=#8E39A8>白天要睡觉</color>\n磁力是一种强大的力量，非常强大，强大到有时都吓到磁力菇自己了。能力越大，责任越大，他不知道自己能否肩负得起这责任。";
				break;
			case PlantType.GoldMagnet:
				MainText.text = "<color=#2B345B>吸金菇在吸取金属物品的同时为你收集硬币和钻石。</color>\n特性：<color=#52BE00>大范围吸取多个僵尸的金属物品。</color>\n范围：<color=#D54130>当前场地的所有钱币和金属物品</color>\n“我怎么在这终老一生” 吸金磁问道，“我曾经也有是平步青云的——坐在办公室的一角，完全的福利啊，股票期权啊。我曾经也是中美公司副总裁啊！现在呢？我却在这，在这个破草地上，时刻担心会被僵尸吃掉！喔~等等，有枚硬币！”";
				break;
			case PlantType.Coffeebean:
				MainText.text = "<color=#2B345B>咖啡豆，可以唤醒睡眠中的蘑菇们。</color>\n用法：<color=#D54130>一次性使用，立即生效</color>\n特点：<color=#D54130>可以种植在其它植物上，用来在白天唤醒蘑菇</color>\n咖啡豆：“嘿，伙计们！嘿，怎么回事？是谁？嘿！你瞧见了那个东西没？什么东西？哇！是狮子！”嗯，咖啡豆确定，这样可以让自己很兴奋。";
				break;
			case PlantType.Pumpkin:
				LoadBackground(3);
				MainText.text = "<color=#2B345B>南瓜头，可以用他的外壳保护其他植物。</color>\n强度：<color=#D54130>高</color>\n特点：<color=#D54130>可以种植在其它植物上</color>\n南瓜头最近都没收到关于他表哥伦菲尔德的消息。很明显，伦菲尔德是个大明星，是一种……叫什么运动来着……的体育明星？幻幻球大师？南瓜头反正搞不懂是什么运动，他只想做好他自己的工作。";
				break;
			case PlantType.Cactus:
				LoadBackground(3);
				MainText.text = "<color=#2B345B>仙人掌发射的穿刺弹可以用来打击地面和空中目标。</color>\n伤害：<color=#D54130>中等</color>\n范围：<color=#D54130>地面和空中</color>\n确实，仙人掌非常“刺儿”，但是她的刺下隐藏着颗温柔的心，充满着爱和善良。 她只是想拥抱别人，和被别人拥抱。大多数人都做不到这点，但是仙人掌她并不介意。她盯着一只铠甲鼠好一阵子了，这次好像真的可以抱抱了。";
				break;
			case PlantType.Starfruit:
				LoadBackground(3);
				MainText.text = "<color=#2B345B>杨桃可以向五个方向发射小星星。</color>\n伤害：<color=#D54130>中等</color>\n范围：<color=#D54130>五个方向</color>\n杨桃：“嘿，哥们儿，有一天我去看牙医，他说我有四个牙洞。我一数，我就只有一颗牙齿！一颗牙齿长了四个牙洞？怎么会这样啊？”";
				break;
			case PlantType.Umbrellaleaf:
				MainText.text = "<color=#2B345B>萝卜伞保护临近的植物不被飞贼僵尸和投石车僵尸所伤害。</color>\n特性：<color=#52BE00>可一次性避雷</color>\n特点：<color=#D54130>保护附近的植物，不被飞贼僵尸和投石车僵尸所伤害</color>\n萝卜伞：“砰！你喜欢这样不，我还可以做给你瞧。砰！哇！我快速向上张开我的叶子，来保护我的邻居。耶！就是这样。没错，就是这样，相信我。";
				break;
			case PlantType.Garlic:
				MainText.text = "<color=#2B345B>大蒜可以让僵尸改变前进的路线。</color>\n范围：<color=#D54130>近距离接触</color>\n特点：<color=#D54130>改变僵尸的前进路线</color>\n路线转向，这不仅仅是大蒜的专业，更是他的热情所在。他在布鲁塞尔大学里获得了转向学的博士学位。他能把路线向量和反击阵列，讲上一整天。他甚至会把家里的东西推到街上去。不知道为啥，他老婆还可以忍受这些。";
				break;
			case PlantType.Cattail:
				LoadBackground(4);
				MainText.text = "<color=#2B345B>攻击任何线上的僵尸以及气球僵尸。</color>\n特性：<color=#52BE00>可以直接种在花盆上，优先攻击离家更近的僵尸</color>\n“嗷！”猫尾草说。“嗷！嗷！嗷！这让你困惑么？你不会就因为我名字里有猫还有我看着像只猫就期待我喵喵喵的叫？这里可不是这样滴~~我可拒绝被随便归类。”";
				break;
			case PlantType.CobCannon:
				MainText.text = "<color=#2B345B>点击玉米加农炮发射致命的玉米炮弹。</color>\n伤害：<color=#D54130>极高</color>\n范围：<color=#D54130>全部场地</color>\n玉米加农炮到底是怎样？嗯~他曾经在哈佛学习，并就职于一家著名的纽约法律事务所。他发射一次就能炸烂大范围的僵尸。这是大家都知道的事。不过往深了想，到底是什么激发了他？";
				break;
			case PlantType.Mint:
				MainText.text = "<color=#2B345B>薄荷可以清除植物的冷却时间。</color>\n范围：<color=#D54130>一格</color>\n他的生活是如此清爽、够劲以至于他在面对僵尸都是一种十分自信的样子。他做事总是十分的迅速，说干就干，周围的植物也会受到他的影响而更加的勤奋";
				break;
			case PlantType.Heronsbill:
				MainText.text = "<color=#2B345B>太阳花可以生产少量的阳光。</color>\n特性：<color=#52BE00>在夜晚产量降低。</color>\n阳光产量：<color=#D54130>低</color>\n“啊~想知道我从哪里来的吗”太阳花边晒太阳边说道我来自一个很温暖的地方，那里吃喝不愁，还有一盆金盏花陪着我简直舒服的不得了，可惜我最终却要被安置在这种地方，每天都有被车子和电线杆砸掉的风险，哦等等，这里也有这么好的阳光？";
				break;
			case PlantType.SnowRepeater:
				MainText.text = "<color=#2B345B>极冻射手可以发射强力减速的冰豌豆。</color>\n伤害：<color=#D54130>中等，具有强大的减速效果</color>\n发射速度：<color=#D54130>两倍</color>\n自从寒冰射手去了北极一次之后就离奇的失踪了，后来当他的同伴们找到他的时候才发现他已经掉入河里变成一大块冰了，当他们把寒冰射手打捞上并且解冻之后发现寒冰射手还离奇的活着，头上的冰也更多了。“这是对我的考验”极冻射手自信的说";
				break;
			case PlantType.Imitater:
				MainText.text = "<color=#2B345B>模仿者允许你在同一个关卡使用两份相同的植物！</color>\n“我还记得76年的那次僵尸战争，”模仿者以沙哑的口吻叙述着，“那时候，我们可没有像豌豆射手和火爆辣椒这样的好伙计。我们只能靠自己的勇气，以及手中的小铲子！”";
				CdText.text = "";
				ValueText.text = "";
				break;
			case PlantType.RottenImitater:
				MainText.text = "<color=#2B345B>随机产生一个植物或僵尸！</color>\n戴夫囤积了太多的货物没来得及卖掉，出了一点意外情况。";
				break;
			case PlantType.Clematis:
				MainText.text = "<color=#2B345B>铁线莲可以吸收大范围的雷电！</color>\n伤害：<color=#D54130>高</color>\n特性：<color=#52BE00>满能量时点击消耗50阳光释放能量球(此时再次引雷会导致死亡)</color>\n铁线莲在幼苗时曾被雷击个半死，但是却奇迹般活了下来，并越长越高进化出了吸引雷电的能力！“从畏惧到喜欢，雷电伤害我的同时也给了我更强的力量。”";
				break;
			case PlantType.Glowstarfruit:
				MainText.text = "<color=#2B345B>荧光杨桃可以对僵尸造成大量的伤害以及强大的控制！</color>\n伤害：<color=#D54130>大，具有强大的眩晕效果</color>\n荧光杨桃非常喜欢那些被吸引并围绕在他周边的萤火虫。“萤火虫和我互相照亮对方，我们互相给予力量和勇气。”";
				break;
			case PlantType.MeltTorch:
				MainText.text = "<color=#2B345B>熔融树桩可以将豌豆变为极强的熔融豌豆</color>\n特性：<color=#52BE00>火焰豌豆会更容易变为熔融豌豆</color>\n";
				break;
			case PlantType.JellyShroom:
				MainText.text = "<color=#2B345B>冻冻菇可以让子弹转向或者弹飞！</color>\n";
				break;
			case PlantType.MoonTombStone:
				LoadBackground(1);
				base.transform.GetComponent<SpriteRenderer>().sprite = ZombieCard;
				MainText.text = "<color=#2B345B>月光墓碑可以生产月光。</color>\n月光产量：<color=#D54130>中等</color>\n僵王博士最新研究的可以聚集月光的神奇石头。";
				break;
			case PlantType.Nope:
				switch (zombieType)
				{
				case ZombieType.NormalZombie:
					MainText.text = "<color=#2B345B>普通僵尸。</color>\n强度：<color=#D54130>低</color>\n这种僵尸喜爱脑髓，贪婪而不知足。脑髓，脑髓，脑髓，夜以继日地追求着。老而臭的脑髓？腐烂的脑髓？都没关系。僵尸需要它们。";
					break;
				case ZombieType.ConeZombie:
					MainText.text = "<color=#2B345B>他的路障头盔，使他两倍坚韧于普通僵尸。</color>\n强度：<color=#D54130>中等</color>\n和其他僵尸一样，路障僵尸盲目地向前。但某些事物却使他停下脚步，捡起一个交通路障，并固实在自己的脑袋上。是的，他很喜欢参加聚会。";
					break;
				case ZombieType.BucketZombie:
					MainText.text = "<color=#2B345B>他的铁桶头盔能极大程度地承受伤害。</color>\n强度：<color=#D54130>高</color>\n弱点：<color=#D54130>磁力菇</color>\n铁桶僵尸经常戴着水桶，部分原因是想让自己在这个冷漠的世界里显得独一无二。但实际上，他经常只是忘记了那铁桶扣在他头上而已。";
					break;
				case ZombieType.DoorZombie:
					MainText.text = "<color=#2B345B>他的铁丝网门是有效的盾牌。</color>\n强度：<color=#D54130>低</color>\n铁丝网门强度：<color=#D54130>高</color>\n弱点：<color=#D54130>大喷菇和磁力菇</color>\n铁网门僵尸上次拜访过的房主防守并不专业，在吃掉那房主的脑子后拿走了他家的铁栅门。";
					break;
				case ZombieType.DoorAndCone:
					MainText.text = "<color=#2B345B>同时装备路障和铁门的强大僵尸。</color>\n强度：<color=#D54130>高</color>\n弱点：<color=#D54130>大喷菇和磁力菇</color>\n这种僵尸对装备有着强烈的追求，更多的装备意味着更高的胜率。";
					break;
				case ZombieType.DoorAndBucket:
					MainText.text = "<color=#2B345B>同时装备铁桶和铁门的强大僵尸。</color>\n强度：<color=#D54130>很高</color>\n弱点：<color=#D54130>磁力菇</color>\n这种僵尸对装备有着强烈的追求，更多的装备意味着更高的胜率。";
					break;
				case ZombieType.Polevaulter:
					MainText.text = "<color=#2B345B>撑杆僵尸利用撑杆跃过障碍物。</color>\n强度：<color=#D54130>中等</color>\n速度：<color=#D54130>快，而后慢（跳跃后）</color>\n特点：<color=#D54130>跃过他所碰到的第一株植物</color>\n一些僵尸渴望走得更远、得到更多，这也促使他们由普通成为非凡。那就是撑杆僵尸。";
					break;
				case ZombieType.PaperZombie:
					LoadBackground(1);
					MainText.text = "<color=#2B345B>他的报纸只能提供有限的防御。</color>\n强度：<color=#D54130>低</color>\n报纸强度：<color=#D54130>低</color>\n伤害：<color=#D54130>高</color>\n速度：<color=#D54130>正常，而后快（失去报纸后）</color>\n读报僵尸,他正痴迷于完成他的数独难题。难怪他这么反常。";
					break;
				case ZombieType.FootballZombie:
					LoadBackground(1);
					MainText.text = "<color=#2B345B>橄榄球僵尸的表演秀。</color>\n强度：<color=#D54130>很高</color>\n速度：<color=#D54130>快</color>\n弱点：<color=#D54130>磁力菇</color>\n在球场上，橄榄球僵尸表现出110 % 的激情。作为一名队员，他进攻防守样样在行。虽然他完全不知道橄榄球是什么。";
					break;
				case ZombieType.BlackFootball:
					LoadBackground(1);
					MainText.text = "<color=#2B345B>黑橄榄球僵尸的表演秀。</color>\n强度：<color=#D54130>极高</color>\n速度：<color=#D54130>快</color>\n弱点：<color=#D54130>大嘴花</color>\n在球场上，橄榄球僵尸表现出210 % 的激情。作为一名队员，他进攻防守样样在行。他似乎知道橄榄球是什么。";
					break;
				case ZombieType.JacksonZombie:
					LoadBackground(1);
					MainText.text = "<color=#2B345B>舞王僵尸和人类如有雷同，纯属巧合。</color>\n强度：<color=#D54130>高</color>\n特殊：<color=#D54130>召唤整列的伴舞僵尸</color>\n舞王僵尸的最新唱片“抓住脑子啃啊啃”在僵尸界的人气正急速飙升。";
					break;
				case ZombieType.Zomboni:
					LoadBackground(2);
					MainText.text = "<color=#2B345B>雪橇车僵尸运用冰雪，碾过你的植物。</color>\n强度：<color=#D54130>高</color>\n特点：<color=#D54130>碾压植物，留下冰道</color>\n不要误会，这不是Zamboni牌浇冰机。Zamboni以及其浇冰机图案都是Frank J. Zamboni & Co., Inc.,的注册商标，而雪橇车僵尸们自然已取得授权。如果你有任何与僵尸无关的浇冰需求，请浏览www.zamboni.com！";
					break;
				case ZombieType.SnorkleZombie:
					LoadBackground(2);
					MainText.text = "<color=#2B345B>潜水僵尸可以潜泳。</color>\n强度：<color=#D54130>弱</color>\n特点：<color=#D54130>潜水避免遭到攻击</color>\n僵尸不呼吸，他们不需要空气，那么为什么潜水僵尸需要用呼吸管来潜泳呢？答案：同行的压力。";
					break;
				case ZombieType.DolphinriderZombie:
					LoadBackground(2);
					MainText.text = "<color=#2B345B>海豚骑手僵尸能用海豚利用你泳池防御中的弱点。</color>\n强度：<color=#D54130>中</color>\n速度：<color=#D54130>快，而后慢</color>\n特点：<color=#D54130>跳过3个植物</color>\n那海豚也是僵尸。";
					break;
				case ZombieType.JackboxZombie:
					LoadBackground(3);
					MainText.text = "<color=#2B345B>这种僵尸带着个会爆炸的弹簧小丑盒子。</color>\n特性：<color=#52BE00>放置者可手动点击引爆</color>\n强度：<color=#D54130>中</color>\n速度：<color=#D54130>快</color>\n特点：<color=#D54130>打开玩偶匣会爆炸</color>\n弱点：<color=#D54130>磁力菇</color>\n这种僵尸令人不寒而栗，不是因为他的冰冷身躯，而是因为他的疯狂。";
					break;
				case ZombieType.BalloonZombie:
					LoadBackground(3);
					MainText.text = "<color=#2B345B>气球僵尸漂浮在空中，躲过大多数攻击。</color>\n强度：<color=#D54130>低</color>\n特点：<color=#D54130>飞行</color>\n弱点：<color=#D54130>仙人掌和三叶草</color>\n气球僵尸真幸运。气球有很多功效，而其他僵尸都不曾捡到过。";
					break;
				case ZombieType.DiggerZombie:
					LoadBackground(3);
					MainText.text = "<color=#2B345B>这种僵尸通过挖隧道来穿过你的防御。</color>\n强度：<color=#D54130>中</color>\n速度：<color=#D54130>快，而后慢</color>\n特点：<color=#D54130>穿过地下隧道并出现在草坪的左侧</color>\n弱点：<color=#D54130>双向射手和磁力菇</color>\n矿工僵尸每周花费三天时间来申请他的挖掘许可证。";
					break;
				case ZombieType.LadderZombie:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>梯子僵尸能爬过障碍物。</color>\n强度：<color=#D54130>中</color>\n梯子强度：<color=#D54130>中</color>\n弱点：<color=#D54130>磁力菇</color>\n这架梯子花了他八块九毛九。";
					break;
				case ZombieType.PogoZombie:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>跳跳僵尸能跳过你的防守。</color>\n强度：<color=#D54130>中</color>\n特点：<color=#D54130>跃过植物</color>\n弱点：<color=#D54130>磁力菇</color>\n蹦！蹦！蹦！那是蹦蹦僵尸发出的有力且有效的声音。";
					break;
				case ZombieType.BungiZombie:
					LoadBackground(6);
					DisplayZombie.transform.position += new Vector3(0f, 0.3f);
					MainText.text = "<color=#2B345B>蹦极僵尸从天上攻击。</color>\n强度：<color=#D54130>中</color>\n特点：<color=#D54130>从天而降窃取你的植物</color>\n蹦极僵尸最爱冒险。毕竟，如果你不再活着，那再体验次死亡又有何妨呢？";
					break;
				case ZombieType.CatapultZombie:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>投石车僵尸操作着重型机器。</color>\n强度：<color=#D54130>中</color>\n特点：<color=#D54130>对你的植物投掷篮球</color>\n投石车僵尸可以利用弹射器来发射任何东西，但篮球似乎是绝佳选择，也是最明显的选择。";
					break;
				case ZombieType.Gargantuar:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>巨人僵尸是一种巨型僵尸。</color>\n强度：<color=#D54130>极高</color>\n当巨人僵尸走动的时候，大地在震颤。当他悲叹的时候，所有的僵尸都沉寂了。他是僵尸们梦想成为的偶像。不过呢，他到现在还没交到女朋友。";
					break;
				case ZombieType.GargantuarHelmet:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>头盔巨人僵尸是一种武装巨型僵尸。</color>\n强度：<color=#D54130>极高</color>\n特点：<color=#D54130>头盔可以让他无视冰冻和黄油</color>\n在巨人僵尸和小鬼僵尸路过一家垃圾场时小鬼僵尸看见了一个巨大的头盔，“看啊！那个头盔正适合你这笨脑袋”巨人僵尸埋怨了小鬼僵尸一句就去拿过头盔带上了，他发现这头盔正适合自己的头，说不定这可以让他找到女朋友，于是他便原谅了小鬼僵尸去受魅惑菇的约会邀请了。";
					break;
				case ZombieType.GargantuarRedeye:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>红眼巨人僵尸是一种暴走巨型僵尸。</color>\n强度：<color=#D54130>极高</color>\n小鬼僵尸通常会在巨人僵尸的背上用餐只是那天小鬼僵尸捡到了一个玉米卷墨西哥玉米卷，沾满辣酱.";
					break;
				case ZombieType.GargantuarHelmetRedeye:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>头盔红眼巨人僵尸是一种极其强力的巨型僵尸。</color>\n强度：<color=#D54130>强大到极点</color>\n特点：<color=#D54130>头盔可以让他无视冰冻和黄油</color>\n相比于头盔巨人僵尸他可就没这么幸运了，他听说头盔巨人僵尸因为一个头盔就找到了女朋友，于是就借了梯子僵尸一大笔钱去打造了和头盔巨人僵尸一样的头盔，不过他在他找到女朋友之后却发现这顶头摘不下来了“实际上是我用僵尸博士的902牌脑子胶水黏上去的”小鬼僵尸笑着说道。";
					break;
				case ZombieType.GargantuarInjured:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>战损巨人僵尸非常的虚弱。</color>\n强度：<color=#D54130>高</color>\n从上一场战斗中存活并取胜。";
					break;
				case ZombieType.ImpZpmbie:
					LoadBackground(6);
					MainText.text = "<color=#2B345B>小鬼僵尸是一群小型僵尸，他们被巨人僵尸扔进你的防御体系。</color>\n强度：<color=#D54130>低</color>\n小鬼僵尸虽然瘦小，却很结实。他精通僵尸柔道，僵尸空手道和僵尸关节技。另外，他还会吹口琴。";
					break;
				case ZombieType.SwampNormal:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>草帽僵尸拥有草帽比其他僵尸更强一点</color>\n强度：<color=#D54130>低</color>\n沼泽草帽僵尸常常草帽傍身，他总觉得一顶漂亮的草帽可以让他和其它僵尸区别开，实际上没人觉得他的草帽好看。";
					break;
				case ZombieType.StoolZombie:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>强大的头顶防御力</color>\n强度：<color=#D54130>中等</color>\n特点：<color=#D54130>能防御大量来自上方的攻击</color>\n那天沼泽草帽僵尸喝醉了，一头扎进了沼泽地的淤泥里，所以他的脑袋现在还卡在这个板凳里面，你说草帽？沼泽草帽僵尸不知道草帽哪里去了。";
					break;
				case ZombieType.SwampBucket:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>沼泽铁桶僵尸拥有一个非常光滑的铁桶</color>\n强度：<color=#D54130>高</color>\n特点：<color=#D54130>无视黄油</color>\n清洗淤泥的时候在小溪里找到了铁桶，“似乎是个非常光滑的草帽呢”，沼泽铁桶僵尸是唯一知道自己戴着什么东西的沼泽僵尸。";
					break;
				case ZombieType.SwampDoor:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>木门僵尸拥有一个强度不高的门板</color>\n强度：<color=#D54130>低</color>\n丛林里常用这种木质的门板，草帽僵尸就把它拆下来举着，比起漂亮的草帽他似乎更想要你家的木门。";
					break;
				case ZombieType.SwampDoorAndStool:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>全方位的保护</color>\n强度：<color=#D54130>中等</color>\n特点：<color=#D54130>能防御大量来自上方的攻击</color>\n那天我和沼泽草帽僵尸打了个赌，我赌他不知道草帽长什么样，所以他就把木门送给了我，就这么简单。";
					break;
				case ZombieType.SwampDoorAndBucket:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>全方位的保护</color>\n强度：<color=#D54130>高</color>\n特点：<color=#D54130>无视黄油</color>\n你知道吗，沼泽的木门也有质量之分,比如草帽僵尸的是中等,板凳木门的是优等,我的则是优等中的优等,“实际上这个门只是他在梯子僵尸不要的垃圾里发现的”。";
					break;
				case ZombieType.Ghost:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>操纵无意识程度的能力</color>\n强度：<color=#D54130>很低</color>\n弱点：<color=#D54130>路灯花</color>\n要感知他需要依赖于外来的能力，“就像路边石子那样的存在”。";
					break;
				case ZombieType.Crocodile:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>水下的王者,空中移动时给后排植物带去鳄运</color>\n强度：<color=#D54130>高</color>\n弱点：<color=#D54130>缠绕海草</color>\n僵尸鳄鱼：我可不是吃素的，可是我鳄了。";
					break;
				case ZombieType.StarveZombie:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>在吃到东西后会逐渐变胖变强</color>\n强度：<color=#D54130>由低变高</color>\n弱点：<color=#D54130>食人花</color>\n饿殍僵尸生前是被撑死的。";
					break;
				case ZombieType.SlimeZombie:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>死亡后分裂成多个小史莱姆僵尸并持续长大</color>\n强度：<color=#D54130>中等</color>\n弱点：<color=#D54130>寒冰菇</color>\n";
					break;
				case ZombieType.SwampGargantuar:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>沼泽巨人僵尸极其强大并会扔出僵尸鳄鱼</color>\n强度：<color=#D54130>极高</color>\n沼泽巨人僵尸是老牌动物保护主义者，因此他不远万里来到沼泽只为照顾僵尸鳄鱼。";
					break;
				case ZombieType.SwampGargantuarBlueEye:
					LoadBackground(7);
					MainText.text = "<color=#2B345B>蓝眼沼泽巨人僵尸极其强大并会扔出僵尸鳄鱼</color>\n强度：<color=#D54130>极高</color>\n速度：<color=#D54130>快</color>\n蓝眼沼泽巨人僵尸在出远门迷路后变的着急狂躁，唯一回去的线索只有出门时带的一块路牌。";
					break;
				case ZombieType.PeaShooterZombie:
					LoadBackground(0);
					MainText.text = "<color=#2B345B>豌豆射手僵尸向前方射出豌豆</color>\n强度：<color=#D54130>低</color>\n一个被植物侵蚀的植物僵尸。";
					break;
				case ZombieType.SnowpeaShooterZombie:
					LoadBackground(0);
					MainText.text = "<color=#2B345B>寒冰射手僵尸向前方射出寒冰豌豆</color>\n强度：<color=#D54130>低</color>\n一个被植物侵蚀的植物僵尸。";
					break;
				case ZombieType.FumeShroomZombie:
					LoadBackground(0);
					MainText.text = "<color=#2B345B>大喷菇僵尸向前方喷出穿透气泡</color>\n强度：<color=#D54130>低</color>\n一个被植物侵蚀的植物僵尸。";
					break;
				case ZombieType.CabbagepultZombie:
					LoadBackground(0);
					MainText.text = "<color=#2B345B>卷心菜僵尸向后方投掷卷心菜</color>\n强度：<color=#D54130>低</color>\n一个被植物侵蚀的植物僵尸。";
					break;
				case ZombieType.Nope:
				case ZombieType.Yeti:
				case ZombieType.PvPTarget:
				case ZombieType.FlagZombie:
				case ZombieType.FlagConeZombie:
				case ZombieType.FlagBucketZombie:
				case ZombieType.FlagSwampZombie:
				case ZombieType.FlagStoolZombie:
				case ZombieType.FlagBucketSwampZombie:
				case ZombieType.SwampGargantuarNoCro:
				case ZombieType.SwampGargantuarBlueEyeNoCro:
				case ZombieType.DancerZombie:
				case ZombieType.DiscoZombie:
				case ZombieType.BackupZombie:
				case ZombieType.SnorkleZombieHelmet:
				case ZombieType.IZombieBrain:
				case ZombieType.BobsledZombie:
				case ZombieType.BobsledZombieHelmet:
				case ZombieType.BobsledZombieSled:
				case ZombieType.BobsledZombieHelmetSled:
				case ZombieType.TubeZombie:
				case ZombieType.TubeConeZombie:
				case ZombieType.TubeBucketZombie:
				case ZombieType.TubeDoorZombie:
				case ZombieType.TubeDoorConeZombie:
				case ZombieType.TubeDoorBucketZombie:
				case ZombieType.RepeaterZombie:
				case ZombieType.GatlingZombie:
				case ZombieType.TorchwoodZombie:
				case ZombieType.WallNutZombie:
				case ZombieType.TallNutZombie:
				case ZombieType.HyponShroomZombie:
				case ZombieType.IceShroomZombie:
				case ZombieType.DoomShroomZombie:
				case ZombieType.JalapenoZombie:
				case ZombieType.SquashZombie:
				case ZombieType.SnowRepeaterZombie:
				case ZombieType.GloomShroomZombie:
				case ZombieType.StarfruitZombie:
				case ZombieType.SunflowerZombie:
				case ZombieType.BloverZombie:
				case ZombieType.UmbrellaZombie:
				case ZombieType.CornpultZombie:
				case ZombieType.MelonpultZombie:
				case ZombieType.WinterMelonZombie:
				case ZombieType.RoadrollerZombie:
				case ZombieType.RockCatapultZombie:
				case ZombieType.HeavyBungiZombie:
					break;
				}
				break;
			}
		}

		public void ClearPlantAndZombie()
		{
			if (DisplayPlant != null)
			{
				Object.Destroy(DisplayPlant.gameObject);
			}
			if (DisplayZombie != null)
			{
				Object.Destroy(DisplayZombie.gameObject);
			}
		}

		private void LoadBackground(int type)
		{
			switch (type)
			{
			case 0:
				MapImg.sprite = NormalMap;
				MapImg.transform.localPosition = new Vector3(0.38f, 0.1f);
				break;
			case 1:
				MapImg.sprite = NightMap;
				MapImg.transform.localPosition = new Vector3(0.38f, 0.1f);
				break;
			case 2:
				MapImg.sprite = DayWaterMap;
				MapImg.transform.localPosition = new Vector3(0.29f, -0.1f);
				break;
			case 3:
				MapImg.sprite = NightWaterMap;
				MapImg.transform.localPosition = new Vector3(0.29f, -0.1f);
				break;
			case 4:
				MapImg.sprite = DayWaterMap;
				MapImg.transform.localPosition = new Vector3(0.29f, 3.3f);
				break;
			case 5:
				MapImg.sprite = NightWaterMap;
				MapImg.transform.localPosition = new Vector3(0.29f, 3.3f);
				break;
			case 6:
				MapImg.sprite = RoofMap;
				MapImg.transform.localPosition = new Vector3(-1.66f, -0.3f);
				break;
			case 7:
				MapImg.sprite = SwampMap;
				MapImg.transform.localPosition = new Vector3(3.2f, 3.3f);
				break;
			}
		}
	}
}
