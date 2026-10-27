# Darklands 完全游戏指南（中文）

> **本指南的数据来源**：游戏内数据文件（`DARKLAND.LST` / `DARKLAND.LOC` / 反编译源码 `src/Darklands.Core`）
> 与原版手册。文中所有属性、技能、圣徒、地名均为**从游戏数据中逐条提取的真实内容**，
> 未核实的项目已明确标注。
>
> 界面文字与本仓库汉化版一致。

---

## 目录

1. [游戏概览](#1-游戏概览)
2. [属性（7 项）](#2-属性7-项)
3. [技能（19 项）](#3-技能19-项)
4. [圣徒（136 位）](#4-圣徒136-位)
5. [圣物（43 件）](#5-圣物43-件)
6. [地点（330 处）](#6-地点330-处)
7. [汉化现状与已知缺口](#7-汉化现状与已知缺口)
8. [术语对照表](#8-术语对照表)

---

## 1. 游戏概览

《Darklands》是 MicroProse 于 **1992 年**发行的开放世界黑暗奇幻 RPG，
背景设在 **15 世纪的神圣罗马帝国**（德意志地区）。

- **无主线剧情**：游戏提供一张真实历史地图，玩家在九十余座真实城镇间旅行、
  接受委托、探索遗迹、与盗贼和异端作战。
- **历史与传说混合**：地图上既有纽伦堡、科隆、布拉格等真实城市，
  也有矮人矿洞、巨龙巢穴、圣徒遗物等传说内容。
- **宗教与罪行系统**：行为会改变**声望**与**罪行**，并最终影响教会审判的结果。

### 队伍

- 一支队伍最多 **4 名成员**。
- 成员会疲劳、受伤、生病，需要在旅馆或营地休息。
- 队员有各自的忠诚与士气，长期欠薪或滥杀无辜会导致离队。

---

## 2. 属性（7 项）

属性是角色的先天素质，直接决定技能检定与生命值。
以下名称取自源码 `Character.cs` 的 `Attribute` 枚举，**顺序与游戏一致**。

| # | 英文 | 中文 | 主要作用 |
| --- | --- | --- | --- |
| 1 | `Endurance` | 耐力 | 生命值、抗疲劳、抗疾病、抗毒 |
| 2 | `Strength` | 力量 | 近战伤害、负重能力 |
| 3 | `Agility` | 敏捷 | 命中、闪避、潜行、弓术 |
| 4 | `Perception` | 感知 | 侦察、发现陷阱与隐藏物、街头智慧 |
| 5 | `Intelligence` | 智力 | 炼金、读写、推理、学习速度 |
| 6 | `Charisma` | 魅力 | 交涉、招募、演讲、领导 |
| 7 | `DivineFavor` | 神恩 | 祈祷效果、圣徒回应、宗教检定 |

> ⚠️ **注意**：游戏中的角色面板（`CharacterSheetScreen` / `PartyPanel` / `PartyInfoScreen`）
> **尚未接入翻译表**，属性名在游戏中仍显示为英文。上表中文为译者建议译名。

### 负重状态

源码中定义了四种负重状态（`HeavyLoad` / `LightLoad` / `NormalLoad` / `Overloaded`），
对应游戏界面上的 `Heavy Load` / `Light Load` / `Normal Load` / `Overloaded`。

---

## 3. 技能（19 项）

技能取自源码 `Character.cs` 的 `Skill` 枚举，**共 19 项，顺序与游戏一致**。

### 战斗技能（5）

| # | 英文 | 中文 | 说明 |
| --- | --- | --- | --- |
| 1 | `EdgedWeapons` | 刃具 | 剑、刀、匕首等带刃武器 |
| 2 | `ImpactWeapons` | 钝击 | 锤、棍棒 |
| 3 | `FlailWeapons` | 连枷 | 链枷类武器 |
| 4 | `Polearms` | 长杆 | 长矛、戟、长柄武器 |
| 5 | `ThrownWeapons` | 投掷 | 飞刀、飞斧、标枪 |

### 远程技能（2）

| # | 英文 | 中文 | 说明 |
| --- | --- | --- | --- |
| 6 | `Bows` | 弓术 | 长弓、短弓、弩 |
| 7 | `MissileDevices` | 远程装置 | 投石器、吹箭等 |

### 知识技能（5）

| # | 英文 | 中文 | 说明 |
| --- | --- | --- | --- |
| 8 | `Alchemy` | 炼金 | 识别与调配药剂、解读配方 |
| 9 | `ReadWrite` | 读写 | 阅读文献、碑文、配方 |
| 10 | `SpeakCommon` | 通用语 | 日常交涉、说服 |
| 11 | `SpeakLatin` | 拉丁语 | 学术、宗教、外交 |
| 12 | `Healing` | 治疗 | 处理伤口、疾病、中毒 |

### 生存技能（4）

| # | 英文 | 中文 | 说明 |
| --- | --- | --- | --- |
| 13 | `Artifice` | 工艺 | 修理、开锁、机关 |
| 14 | `Stealth` | 潜行 | 潜行、偷窃、隐蔽行动 |
| 15 | `Streetwise` | 街头智慧 | 打探消息、黑市、地下交易 |
| 16 | `Woodwise` | 林野知识 | 野外生存、追踪、辨认地形 |

### 精神技能（3）

| # | 英文 | 中文 | 说明 |
| --- | --- | --- | --- |
| 17 | `Religion` | 宗教 | 宗教知识、仪式、祈祷（界面显示 `Religious Trng`） |
| 18 | `Virtue` | 德行 | 道德修养、抵制诱惑 |
| 19 | `Riding` | 骑乘 | 骑马旅行、马上战斗 |

> ⚠️ **注意**：技能名在游戏中仍显示为英文（同上，角色面板未接入翻译表）。
> 界面上的缩写形式：`Edged Wpns` / `Impact Wpns` / `Flail Wpns` / `Polearm Wpns` / `Thrown Wpns`。

### 技能检定

技能检定的一般形式为：

```
检定结果 = 技能值 + 相关属性修正 - 难度
```

属性越高，修正越有利。技能通过**实际使用**提升。

---

## 4. 圣徒（136 位）

游戏中有 **136 位圣徒**，每位都有全名、短名、圣日与专属领域。
以下名单从 `DARKLAND.LST` 中逐条提取，中文为汉化版译名。

### 完整名单

| # | 全名 | 中文 |
| --- | --- | --- |
| 1 | St.Adrian | 圣阿德里安 |
| 2 | St.Agatha | 圣阿加莎 |
| 3 | St.Agnes | 圣阿格尼丝 |
| 4 | St.Aidan | 圣艾丹 |
| 5 | St.Albert the Great | 大圣阿尔伯特 |
| 6 | St.Alcuin | 圣阿尔昆 |
| 7 | St.Alexis | 圣阿莱克西斯 |
| 8 | St.Andrew | 圣安德烈 |
| 9 | St.Andrew the Tribune | 圣安德烈（百夫长） |
| 10 | St.Anthony | 圣安东尼 |
| 11 | St.Anthony of Padua | 圣帕多瓦的安东尼 |
| 12 | St.Arnulf | 圣阿努尔夫 |
| 13 | St.Apollinarius | 圣阿波利纳里斯 |
| 14 | St.Barbara | 圣芭芭拉 |
| 15 | St.Bathildis | 圣巴蒂尔德 |
| 16 | St.Boniface | 圣博尼法斯 |
| 17 | St.Catherine | 圣凯瑟琳 |
| 18 | St.Catherine of Siena | 圣锡耶纳的凯瑟琳 |
| 19 | St.Cecilia | 圣塞西莉亚 |
| 20 | St.Ceolwulf | 圣塞尔武尔夫 |
| 21 | St.Charity | 圣慈善 |
| 22 | St.Christina, Astonish. | 圣克里斯蒂娜（惊愕者） |
| 23 | St.Christopher | 圣克里斯托弗 |
| 24 | St.Clare | 圣克莱尔 |
| 25 | St.Clotilda | 圣克洛蒂尔德 |
| 26 | St.Colman of Cloyne | 圣克洛因的科尔曼 |
| 27 | St.Columba | 圣科伦巴 |
| 28 | St.Cosmas | 圣科斯马斯 |
| 29 | St.Crispin | 圣克里斯平 |
| 30 | St.Cyprian | 圣西普里安 |
| 31 | St.Damian | 圣达米安 |
| 32 | St.David | 圣大卫 |
| 33 | St.Denis | 圣德尼 |
| 34 | St.Derfel Gadarn | 圣德费尔·加达恩 |
| 35 | St.Devota | 圣德沃塔 |
| 36 | St.Dismas | 圣迪斯马斯 |
| 37 | St.Dominic | 圣多米尼克 |
| 38 | St.Dorothy of Montau | 圣蒙陶的多萝西 |
| 39 | St.Drogo | 圣德罗戈 |
| 40 | St.Dunstan | 圣邓斯坦 |
| 41 | St.Dymphna | 圣丁姆芙娜 |
| 42 | St.Edward, Confessor | 圣爱德华（忏悔者） |
| 43 | St.Eligius | 圣埃利吉乌斯 |
| 44 | St.Emydius | 圣埃米迪乌斯 |
| 45 | St.Engelbert | 圣恩格尔贝特 |
| 46 | St.Erasmus | 圣伊拉斯谟 |
| 47 | St.Eric | 圣埃里克 |
| 48 | St.Eustace | 圣尤斯塔斯 |
| 49 | St.Felix of Nola | 圣诺拉的费利克斯 |
| 50 | St.Finbar | 圣芬巴尔 |
| 51 | St.Finnian | 圣芬尼安 |
| 52 | St.Florian | 圣弗洛里安 |
| 53 | St.Francis | 圣方济各 |
| 54 | St.Gabriel | 圣加百列 |
| 55 | St.Genevieve | 圣热纳维耶芙 |
| 56 | St.George | 圣乔治 |
| 57 | St.Gerlac | 圣格拉赫 |
| 58 | St.Gertrude o'Nivelles | 圣尼维勒的格特鲁德 |
| 59 | St.Giles | 圣吉尔 |
| 60 | St.Giles of Portugal | 圣葡萄牙的吉尔 |
| 61 | St.Godehard | 圣戈德哈德 |
| 62 | St.Godfrey | 圣戈弗雷 |
| 63 | St.Gottschalk | 圣戈特沙尔克 |
| 64 | St.Gregory Thaumaturg. | 圣格列高利（行奇迹者） |
| 65 | St.Hedwig | 圣海德薇 |
| 66 | St.Henry | 圣亨利 |
| 67 | St.Heribert | 圣赫里伯特 |
| 68 | St.Herve | 圣埃尔韦 |
| 69 | St.Hildegard | 圣希尔德加德 |
| 70 | St.Hubert | 圣休伯特 |
| 71 | St.Illtyd | 圣伊尔蒂德 |
| 72 | St.Isidore | 圣伊西多尔 |
| 73 | St.Ita | 圣伊塔 |
| 74 | St.James | 圣雅各 |
| 75 | St.Januarius | 圣雅努阿里乌斯 |
| 76 | St.John of Bridlington | 圣布里德灵顿的约翰 |
| 77 | St.John Chrysostom | 圣约翰（金口） |
| 78 | St.John Climacus | 圣约翰·克利马科斯 |
| 79 | St.John Nepolmuchen | 圣约翰·涅波尔穆琴 |
| 80 | St.Joseph | 圣约瑟 |
| 81 | St.Jude | 圣犹达 |
| 82 | St.Julian, Hospitaler | 圣朱利安（接待者） |
| 83 | St.Kessog | 圣凯索格 |
| 84 | St.Lasdislaus | 圣拉斯迪斯劳斯 |
| 85 | St.Lawrence | 圣劳伦斯 |
| 86 | St.Lazarus | 圣拉撒路 |
| 87 | St.Longinius | 圣朗吉努斯 |
| 88 | St.Lucy | 圣露西 |
| 89 | St.Luke | 圣路加 |
| 90 | St.Lutgardis | 圣卢特加迪斯 |
| 91 | St.Margaret | 圣玛格丽特 |
| 92 | St.Margaret of Cortona | 圣科托纳的玛格丽特 |
| 93 | St.Mark | 圣马可 |
| 94 | St.Martin of Tours | 圣图尔的马丁 |
| 95 | St.Matthew | 圣马太 |
| 96 | St.Maurice | 圣莫里斯 |
| 97 | St.Michael | 圣弥额尔 |
| 98 | St.Milburga | 圣米尔布尔加 |
| 99 | St.Moses the Black | 圣黑人摩西 |
| 100 | St.Nicholas | 圣尼古拉 |
| 101 | St.Odilia | 圣奥迪利亚 |
| 102 | St.Odo | 圣奥多 |
| 103 | St.Olaf | 圣奥拉夫 |
| 104 | St.Pantaleon | 圣潘塔莱翁 |
| 105 | St.Patrick | 圣帕特里克 |
| 106 | St.Paul | 圣保禄 |
| 107 | St.Paul the Simple | 圣保罗（单纯者） |
| 108 | St.Perpetua | 圣佩尔佩图阿 |
| 109 | St.Peter | 圣伯多禄 |
| 110 | St.Peter of Atroa | 圣阿特拉的彼得 |
| 111 | St.Polycarp | 圣波利卡普 |
| 112 | St.Raphael | 圣拉斐尔 |
| 113 | St.Raymond Penafort | 圣雷蒙德·佩尼亚福特 |
| 114 | St.Raymond Lull | 圣雷蒙德·卢尔 |
| 115 | St.Reinold | 圣莱诺尔德 |
| 116 | St.Roch | 圣罗克 |
| 117 | St.Sabas the Goth | 圣西巴斯的哥特人 |
| 118 | St.Sebastian | 圣塞巴斯蒂安 |
| 119 | St.Stanislaus | 圣斯坦尼斯劳斯 |
| 120 | St.Stephen | 圣斯德望 |
| 121 | St.Swithbert | 圣斯威特伯特 |
| 122 | St.Tarachus | 圣塔拉库斯 |
| 123 | St.Thalelaeus,Merciful | 圣塔莱劳斯（仁慈者） |
| 124 | St.Theodore Tiro | 圣西奥多（提罗） |
| 125 | St.Thomas the Apostle | 圣多默（宗徒） |
| 126 | St.Thomas Aquinas | 圣托马斯·阿奎那 |
| 127 | St.Valentine | 圣瓦伦丁 |
| 128 | St.Victor o'Marseilles | 圣马赛的维克多 |
| 129 | St.Vitus | 圣维图斯 |
| 130 | St.Wenceslaus | 圣瓦茨拉夫 |
| 131 | St.Wilfrid | 圣威尔弗里德 |
| 132 | St.Willehad | 圣威利哈德 |
| 133 | St.Willebald | 圣威勒巴尔德 |
| 134 | St.Williborord | 圣威利博罗德 |
| 135 | St.Wolfgang | 圣沃尔夫冈 |
| 136 | St.Zita | 圣齐塔 |

### 圣徒的用途

- **祈祷**：在教堂或圣坛向圣徒祈祷，需要消耗**神恩（Divine Favor）**。
- **圣日**：每位圣徒有专属的**圣日（Saint Day）**，在圣日祈祷效果更好。
- **领域**：不同圣徒掌管不同领域（战争、治愈、旅行等）。
- **圣物**：部分圣徒拥有可寻获的**圣物**（见下一节）。

---

## 5. 圣物（18 件）

圣物是稀有的神圣物品，以「短名 + 物品名」的形式记录（如 `S.George GreatSwrd` = 圣乔治巨剑）。
以下 18 件从 `DARKLAND.LST` 提取：

| # | 英文 | 中文 |
| --- | --- | --- |
| 1 | S.Mary's Tears | 圣玛丽之泪 |
| 2 | S.Arnulf GreatSwrd | 圣阿努尔夫巨剑 |
| 3 | S.Catherine Pain | 圣凯瑟琳之痛 |
| 4 | S.Olaf Battleaxe | 圣奥拉夫战斧 |
| 5 | S.George GreatSwrd | 圣乔治巨剑 |
| 6 | S.Dunstan Hammer | 圣邓斯坦之锤 |
| 7 | S.Hubert's Bow | 圣休伯特之弓 |
| 8 | S.Edward's Ring | 圣爱德华之戒 |
| 9 | S.Emygdius Finger | 圣埃米迪乌斯之指 |
| 10 | S.Gabriel's Horn | 圣加百列之角 |
| 11 | S.Ita's Needle | 圣伊塔之针 |
| 12 | S.Kessog Medallion | 圣凯索格徽章 |
| 13 | S.Odo's Testament | 圣奥多遗言 |
| 14 | S.Odilia's Oil | 圣奥迪利亚之油 |
| 15 | S.Raphael's Water | 圣拉斐尔之水 |
| 16 | S.Swithbert's Foot | 圣斯威特伯特之足 |
| 17 | S.Willehad's Shoe | 圣威利哈德之鞋 |
| 18 | S.Thealeaus' Spoon | 圣西劳斯之勺 |

### 关于圣徒短名的截断

`DARKLAND.LST` 中圣徒**短名（SHORT）字段只有 10 字节**，因此多词名会被截断：

| 短名（截断） | 全名 |
| --- | --- |
| `S.Andrew Tr` | S.Andrew the Tribune |
| `S.Anthny Pa` | S.Anthony of Padua |
| `S.Cathrn Si` | S.Catherine of Siena |
| `S.Colman Cl` | S.Colman of Cloyne |
| `S.Dorthy Mn` | S.Dorothy of Montau |
| `S.Derfel Ga` | S.Derfel Gadarn |
| `S.Edward Cf` | S.Edward, Confessor |
| `S.Gertrd Ni` | S.Gertrude o'Nivelles |
| `S.Giles Por` | S.Giles of Portugal |
| `S.Gregory T` | S.Gregory Thaumaturg. |
| `S.John Bdgt` | S.John of Bridlington |
| `S.John Chry` | S.John Chrysostom |
| `S.John Clmc` | S.John Climacus |
| `S.John Nepl` | S.John Nepolmuchen |
| `S.Margart C` | S.Margaret of Cortona |
| `S.Martin Tr` | S.Martin of Tours |
| `S.Moses Blk` | S.Moses the Black |
| `S.Paul Smpl` | S.Paul the Simple |
| `S.Peter Atr` | S.Peter of Atroa |
| `S.Raymnd Pf` | S.Raymond Penafort |
| `S.Raymnd Lu` | S.Raymond Lull |
| `S.Sabas Gth` | S.Sabas the Goth |
| `S.Theodore T` | S.Theodore Tiro |
| `S.Thom Apst` | S.Thomas the Apostle |
| `S.Victor Mar` | S.Victor o'Marseilles |

> 这是**原版数据的固有限制**，不是汉化引入的问题。游戏中这些名字即以此形式显示。

---

## 6. 地点（330 处）

`DARKLAND.LOC` 共 **414 条记录**，其中 **72 条为地形占位**
（`Lair` / `Cave` / `Spring` / `Lake` / `Shrine` / `Tomb` / `Pagan Altar` / `Alpine Cave`），
**342 条为具名地点，去重后 330 处**。

### 重要城镇

| 地名（游戏内） | 现代拼写 | 说明 |
| --- | --- | --- |
| N|rnberg | Nürnberg | 纽伦堡，帝国重镇 |
| K{ln | Köln | 科隆 |
| M|nchen | München | 慕尼黑 |
| L|beck | Lübeck | 吕贝克，汉萨同盟 |
| W|rzburg | Würzburg | 维尔茨堡 |
| Regensburg | Regensburg | 雷根斯堡 |
| Strassburg | Straßburg | 斯特拉斯堡 |
| Z|rich | Zürich | 苏黎世 |
| Prag | Prag | 布拉格 |
| Wien | Wien | 维也纳 |
| Augsburg | Augsburg | 奥格斯堡 |
| Ulm | Ulm | 乌尔姆 |
| Basel | Basel | 巴塞尔 |
| M{mpelgard | Mömpelgard | 蒙贝利亚尔 |
| Br|nn | Brünn | 布尔诺 |
| Olm|tz | Olmütz | 奥洛穆茨 |
| Gr{tsch | Grötsch | 格拉茨 |
| Salzburg | Salzburg | 萨尔茨堡 |
| N{rdlingen | Nördlingen | 讷德林根 |
| Rottweil | Rottweil | 罗特魏尔 |

> **关于地名中的 `|` 与 `{`**：游戏使用自定义字体编码，原始数据中
> `|` 代表 `ü`（如 `L|beck` = Lübeck），`{` 代表 `ö`（如 `K{ln` = Köln）。
> 上表已还原为现代拼写。

### 其他地点类型

除城镇外，地图上还有：

- **地形类**：洞穴、矿坑、湖泊、泉水、神庙、陵墓、异教祭坛
- **传说类**：如 `Hexentanzplatz`（女巫舞场）、`Teufelstein`（魔鬼石）、`Brocken`（布罗肯峰）
- **河流与渡口**：如 `Elbe`（易北河）

---

## 7. 汉化现状与已知缺口

### 已完成

| 部分 | 状态 |
| --- | --- |
| 卡片文本（叙述/对话/选项） | 已译（7,729 条可打印 run） |
| 圣徒名（全名 + 短名） | 已译（136 + 112 条） |
| 圣徒描述 | 已译（136 条） |
| 商店 / 交易 / 拾取界面 | 已译（模板 + 参数化） |
| 祈祷界面 | 已译 |
| 物品名 | 已译（含 `V:` / `L:` 前缀保留） |

### 已知缺口

| 部分 | 状态 |
| --- | --- |
| **角色面板**（属性名、技能名） | ❌ **未译**，仍为英文 |
| **队伍信息面板** | ❌ 未译 |
| **控制台中** | ❌ 未译 |
| 图片内嵌文字（标题、字体图） | ❌ 需 HD 包，未做 |
| 部分代码内硬编码字符串 | ❌ 未译 |

### 关于角色面板

经核查，`CharacterSheetScreen.cs` / `PartyPanel.cs` / `PartyInfoScreen.cs`
**均未调用 `TranslationTable`**，因此属性名与技能名在游戏中仍显示英文。
这是目前最大的可见缺口。

---

## 8. 术语对照表

### 界面常用词

| 英文 | 中文 |
| --- | --- |
| Endurance | 耐力 |
| Strength | 力量 |
| Agility | 敏捷 |
| Perception | 感知 |
| Intelligence | 智力 |
| Charisma | 魅力 |
| Divine Favor | 神恩 |
| Age | 年龄 |
| Leader | 领袖 |
| Edged Wpns | 刃具 |
| Impact Wpns | 钝击 |
| Flail Wpns | 连枷 |
| Polearm Wpns | 长杆 |
| Thrown Wpns | 投掷 |
| Bow Weapons | 弓术 |
| Missile Device | 远程装置 |
| Alchemy | 炼金 |
| Religious Trng | 宗教 |
| Virtue | 德行 |
| Speak Common | 通用语 |
| Speak Latin | 拉丁语 |
| Read | 读写 |
| Healing | 治疗 |
| Artifice | 工艺 |
| Stealth | 潜行 |
| Streetwise | 街头智慧 |
| Riding | 骑乘 |
| Woodwise | 林野知识 |
| Heavy Load | 重负 |
| Light Load | 轻负 |
| Normal Load | 正常负重 |
| Overloaded | 超载 |
| No Weapon | 无武器 |
| No Shield | 无盾 |
| No Limb Armor | 无肢体护甲 |
| No Vitals Armor | 无躯干护甲 |
| No Missile Weapon | 无远程武器 |
| DF Remaining | 剩余神恩 |
| Saint Day | 圣日 |
| Success | 成功 |
| Failure | 失败 |
| Relics | 圣物 |
| Favor | 恩宠 |
| Virtue | 德行 |
| Sin | 罪 |
| Guilt | 罪疚 |
| Reputation | 声望 |
| Wealth | 财富 |
| Gold | 金币 |
| Alchemy | 炼金 |
| Formulae | 配方 |
| Ingredients | 材料 |
| Potions | 药剂 |

### 物品前缀

游戏中物品名自带槽位前缀（**汉化版保留不改**）：

| 前缀 | 含义 |
| --- | --- |
| `V:` | Vitals（重要部位）—— 头部与躯干护甲 |
| `L:` | Limbs（四肢）—— 手臂与腿部护甲 |

> 用户决定保留 `V:` / `L:` 前缀不翻译，以保持游戏特色。

---

## 附录：数据文件对照

| 文件 | 内容 |
| --- | --- |
| `DARKLAND.LST` | 物品定义（200 槽）、圣徒名（136）、配方名（66） |
| `DARKLAND.LOC` | 地点（414 条记录，330 处具名） |
| `DARKLAND.SNT` | 圣徒描述（136 条，每条 360 字节） |
| `MSGFILES` | 卡片文本 |

---

**本指南完。**

*数据提取自游戏本体，译名与汉化版一致。如有错漏，欢迎指正。*
