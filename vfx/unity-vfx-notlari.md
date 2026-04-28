# Unity Game Development Pathway - VFX Notlari

Unity Learn Game Development Pathway, Unit 6 - Visual Effects icin ozet ve pratik notlar.

Unity version: 6.3  
VFX Graph package: 17.6  
Hazirlanma tarihi: 2026-04-28

## Kisa Ozet

Unity VFX, kucuk gorselleri veya mesh'leri zaman icinde spawn edip hareket, renk, boyut ve render davranisi vererek efekt uretmektir.

Ornekler:

- Fire
- Smoke
- Rain
- Snow
- Explosion
- Pickup sparkle
- Magic dust
- Slash trail
- Dust trail

Unity'de ana iki VFX sistemi vardir:

| Sistem | Ne zaman kullanilir? |
|---|---|
| Built-in Particle System | Basit ve orta seviye efektler, gameplay feedback, physics interaction, genis platform destegi |
| VFX Graph | Cok yogun particle, GPU tabanli efektler, milyonlarca particle, URP/HDRP projeleri |

## Genel VFX Mantigi

Bir efekt genelde su pipeline ile dusunulur:

```text
Oyun olayi / emitter
        |
        v
Emission / Spawn
Kac particle? Ne zaman?
        |
        v
Initialize / Main + Shape
Nereden dogsun? Hizi, lifetime, size, color ne olsun?
        |
        v
Update over Lifetime
Yasarken nasil degissin?
Noise, gravity, force, color, size, rotation, texture frame...
        |
        v
Renderer / Output
Nasil gorunsun?
Material, texture, billboard, sorting, transparency...
```

Particle System'de bunlar Inspector icindeki module'lar olarak gorunur. VFX Graph'ta bunlar Context, Block ve Operator node'lari olarak gorunur.

## VFX Turleri

| Tur | Ornek | Amac |
|---|---|---|
| Environmental effects | Fire, rain, snow, fog, mist | Ortama canlilik ve atmosfer verir |
| Gameplay effects | Pickup sparkle, hit flash, slash trail, explosion | Oyuncuya feedback verir |

VFX sadece guzel gorunmek icin degil, oyuncuya bir sey oldugunu anlatmak icin kullanilir.

## Unit Section Ozetleri

## 0. Introduction to Unit 6

Bu unit'in hedefi:

- VFX'in oyuna polish ve feedback kattigini anlamak.
- Fire, weather ve smoke burst gibi efektleri Particle System ile yapmak.
- VFX Graph'i tanimak.
- Sonra kendi 2D veya 3D oyununa efekt eklemek.

Kendi oyununda beklenen minimum VFX'ler:

- Pickup alindiginda particle effect.
- Enemy/player collision oldugunda particle effect.
- 2D projede environment particles veya collision feedback.

## 1. Get Started with VFX

Bu bolumde VFX'in ne oldugu anlatilir.

Ornekler:

- Su ustu mist: ortami gizemli yapar.
- Weapon slash trail: saldirinin yonunu ve etkisini gosterir.
- Confetti veya pickup burst: basari feedback'i verir.

Onemli fikir:

```text
VFX = atmosfer + feedback + polish
```

## 2. Play Around with a Particle System

Hazir bir fire effect incelenir.

Onemli ders:

```text
Kompleks bir efekt genelde tek Particle System degildir.
Birkac kucuk Particle System'in birlesimidir.
```

Ornek fire prefab yapisi:

```text
Fire_ParticleSystem_Prefab
|-- Flames
|-- Smoke
`-- Sparks
```

Sparks aktif edilince ates daha canli gorunur.

Bu bolumde gorulen module'lar:

| Module | Ne yapar? |
|---|---|
| Emission | Kac particle cikacak? |
| Shape | Nereden cikacak? Cone, Sphere, Box, Circle vb. |
| Renderer | Nasil gorunecek? Material, billboard, sorting |
| Color over Lifetime | Yaslandikca rengi veya alpha'si degissin |
| Size over Lifetime | Yaslandikca buyusun veya kuculsun |
| Noise | Harekete randomness ve turbulence katsin |

## 3. Create an Environmental Particle System

Snow veya rain gibi surekli calisan environment effect yapilir.

Temel kurulum:

```text
GameObject > Effects > Particle System
Name: FX_Snow veya FX_Rain
Position: X 0, Y 10, Z 0
Rotation: X 90, Y 0, Z 0
```

Main module ayarlari:

| Property | Ne ise yarar? |
|---|---|
| Start Size | Particle boyutu. Snow/rain icin kucuk degerler kullanilir |
| Start Speed | Ilk hiz. Snow yavas, rain hizli olur |
| Start Lifetime | Particle kac saniye yasayacak |
| Prewarm | Scene baslar baslamaz yagmur/kar dolu gorunsun |
| Max Particles | Ayni anda en fazla kac particle olabilir |

Onemli formuller:

```text
Ekrandaki particle sayisi ~= Rate over Time x Start Lifetime
Travel distance ~= Start Speed x Start Lifetime
```

Shape module:

```text
Shape: Box
Scale: X 10, Y 10, Z 1
Emit from: Volume
```

Emission module:

```text
Rate over Time: 150 - 1000
```

Renderer module:

- SnowMaterial
- RaindropMaterial
- SmokeMaterial
- SparkMaterial

Particle texture'lari transparent background'lu olmali.

## 4. Create a Burst Particle

Bu bolumde event-based smoke burst yapilir.

Fark:

| Continuous effect | Burst effect |
|---|---|
| Rain, fire, snow | Explosion, smoke puff, pickup sparkle |
| Surekli calisir | Bir olayda tek sefer patlar |

Smoke burst setup:

```text
Name: FX_SmokeBurst
Position: X 0, Y 0.5, Z 0
Looping: Off
Play On Awake: Off
Emission Rate over Time: 0
Bursts: + add burst
Time: 0
Count: 30
Cycles: 1
Probability: 1
```

Smoke movement:

```text
Start Speed: ~0.3
Start Lifetime: ~1.0
Shape: Sphere
Radius: ~0.5
```

Randomness icin bircok property dropdown'undan su mod secilebilir:

```text
Random Between Two Constants
```

Ornek:

```text
Start Lifetime: 0.5 - 1.5
Start Speed: 0.2 - 0.5
```

VFX'te her particle ayni davranirsa efekt yapay gorunur. Randomness bu yuzden cok onemlidir.

Color over Lifetime icin smoke mantigi:

```text
transparent -> visible -> transparent
```

Gradient editor:

- Ust marker'lar alpha'yi kontrol eder.
- Alt marker'lar color'i kontrol eder.

Texture Sheet Animation:

```text
Tiles: X 2, Y 2
Start Frame: Random Between Two Constants 0 - 3
Cycles: 0 veya cok dusuk
```

Bu, her particle'in farkli smoke frame kullanmasini saglar.

Size over Lifetime icin iyi smoke curve:

```text
size
  ^
1 |      /\
  |     /  \
0 |____/    \____> lifetime
```

Yani kucuk basla, buyu, sonra kucul veya fade out.

## 5. Experiment with VFX Graph

VFX Graph, Unity'nin ikinci VFX sistemidir.

Particle System ve VFX Graph karsilastirmasi:

| Konu | Built-in Particle System | VFX Graph |
|---|---|---|
| Workflow | Inspector modules | Node graph |
| Simulasyon | CPU / C# erisimli | GPU |
| Particle sayisi | Thousands | Millions |
| Ogrenme | Daha kolay | Daha zor |
| Physics | Unity physics ile daha iyi etkilesim | Normal Rigidbody/Collider physics gibi calismaz |
| Pipeline | Built-in RP, URP, HDRP | URP, HDRP |
| Platform | Daha genis | Compute shader gerekir |

Kisa karar:

```text
Basit, oyun event'i, physics, mobile, beginner => Particle System
Cok yogun, GPU-based, milyonlarca particle, complex simulation => VFX Graph
```

VFX Graph context'leri yukaridan asagi okunur:

```text
Spawn
  |
Initialize Particle
  |
Update Particle
  |
Output Particle
```

| Context | Particle System karsiligi | Gorevi |
|---|---|---|
| Spawn | Emission | Kac particle dogacak? |
| Initialize | Main + Shape | Ilk position, velocity, lifetime, size |
| Update | Over Lifetime modules | Yasarken nasil degisecek? |
| Output | Renderer | Nasil cizilecek? |

VFX Graph kavramlari:

- Context: Buyuk islem asamasi.
- Block: Context icine eklenen davranis.
- Operator: Matematik veya logic node'u.
- Blackboard: Disa expose edilen parameter'lar.

Unity 6.3 / VFX Graph 17.6 guncel notlar:

- VFX Graph GPU'da simule eder.
- URP ve HDRP ile uyumludur.
- Built-in Render Pipeline icin uygun degildir.
- Compute shader ve SSBO support gerekir.
- OpenGL ES desteklenmez.
- URP'de gamma color space desteklenmez; Linear Color Space kullan.
- Mobile hedefliyorsan mutlaka hedef cihazda test et.

## 6. Challenge: Add Some Magic to Your Scene

Gorev: Sahneye magical veya whimsical effect eklemek.

Kriterler:

- Scene daha magical hissettirmeli.
- Randomness icermeli.
- Color over Lifetime veya alpha change olmali.

Iyi fikirler:

- Fireflies
- Shooting stars
- Magical dust
- Floating glowing orbs
- Sparkle field
- Purple/blue aura

Firefly icin ornek ayar mantigi:

```text
Emission Rate: dusuk
Shape: Box veya Sphere
Start Speed: dusuk
Start Lifetime: random
Color over Lifetime: yellow/green -> transparent
Noise: acik
Size over Lifetime: hafif pulse
Material: additive glowing particle
```

## 7. VFX for 2D Projects

Ana fikir:

```text
Particle System ve VFX Graph 2D projelerde de kullanilabilir.
Mantik ayni, ama bazi 2D-specific ayarlar onemlidir.
```

Ayni kalanlar:

- Emission
- Shape
- Color over Lifetime
- Size over Lifetime
- Noise
- Renderer
- VFX Graph contexts

2D'de farkli olanlar:

| Ayar | Neden onemli? |
|---|---|
| Gravity Source: 2D Physics | 2D projede daha uygun ve optimize |
| Shape: Circle / Rectangle / Sprite | 3D emitter yerine 2D emitter kullan |
| Sorting Layer / Order in Layer | Particle'in sprite'larin onunde veya arkasinda cizilmesini kontrol eder |
| Camera alignment | 2D kamera yonune gore particle duzlemini hizala |
| Z position | 2D'de gorunurluk ve siralama sorunlarina sebep olabilir |

2D'de en sik hata:

```text
Particle calisiyor ama gorunmuyor.
```

Muhtemel sebepler:

- Sorting Layer yanlis.
- Order in Layer yanlis.
- Z position yanlis.
- Material alpha sifir.
- Camera clipping veya layer ayari yanlis.

## 8. Add VFX to Your Game

Bu bolumde ogrendiklerini kendi oyununa eklersin.

Tasarim sorulari:

- Oyunda hangi event'ler feedback ister?
- Pickup alinca ne olmali?
- Enemy carpisinca explosion, smoke veya flash olmali mi?
- Player hareket edince dust trail olmali mi?
- Win durumunda celebration effect olmali mi?
- Efekt atmosfer mi yaratacak, gameplay feedback mi verecek?

3D game minimum oneriler:

- Pickup effect.
- Enemy catches player: explosion, smoke veya death burst.
- Optional: win celebration.
- Optional: player movement dust/smoke trail.

2D game minimum oneriler:

- Environment particles.
- Enemy/player collision effect.
- Theme'e gore custom particles.

## Particle System Module Rehberi

## Main Module

Particle'in temel dogum ayarlaridir.

| Property | Mental model |
|---|---|
| Duration | System cycle uzunlugu |
| Looping | Cycle tekrar etsin mi? |
| Prewarm | Baslangicta dolu simulasyonla basla |
| Start Lifetime | Particle kac saniye yasar |
| Start Speed | Ilk hiz |
| Start Size | Ilk boyut |
| Start Color | Ilk renk |
| Gravity Modifier | Gravity etkisi |
| Simulation Space | Local mi World mu? |
| Play on Awake | Scene baslar baslamaz oynasin mi? |
| Max Particles | Ayni anda maksimum particle |

Onemli:

```text
Travel distance ~= Start Speed x Start Lifetime
```

Rain icin hizli speed ve kisa/orta lifetime. Snow icin dusuk speed ve uzun lifetime.

## Emission Module

Ne kadar particle uretilecegini belirler.

| Property | Kullanim |
|---|---|
| Rate over Time | Saniyede particle |
| Rate over Distance | Hareket ettikce particle; dust trail icin cok iyi |
| Bursts | Belirli anda patlama veya puff |

Burst effect icin:

```text
Rate over Time = 0
Bursts Count = 20-100
Looping = Off
Play On Awake = Off
```

## Shape Module

Particle nereden dogacak?

| Shape | Kullanim |
|---|---|
| Cone | Fire, fountain, upward smoke |
| Sphere | Explosion, puff |
| Box | Rain, snow, fog volume |
| Circle | 2D puff, aura |
| Mesh/Sprite | Objeden veya sprite'tan particle cikarma |

Shape, particle'in kendi sekli degildir. Shape, emitter'in seklidir.

## Renderer Module

Particle'in nasil cizilecegini belirler.

| Ayar | Ne ise yarar? |
|---|---|
| Render Mode: Billboard | Particle kameraya doner |
| Stretched Billboard | Rain streak, speed line |
| Mesh | Particle olarak mesh ciz |
| Material | Texture/shader sec |
| Sorting Layer / Order | 2D siralama |
| Sort Mode | Transparent particle sirasi |

Genel kullanim:

- Smoke: Alpha transparent material.
- Fire/magic: Additive material.
- Rain: Stretched Billboard.
- Snow: Billboard.

## Color over Lifetime

Particle yaslandikca rengi veya alpha'si degisir.

Smoke ornegi:

```text
Alpha 0 -> Alpha 1 -> Alpha 0
```

Fire ornegi:

```text
yellow -> orange -> red -> transparent
```

Magic ornegi:

```text
blue -> purple -> transparent
```

## Size over Lifetime

Particle boyutu lifetime boyunca curve ile degisir.

Explosion ornegi:

```text
kucuk -> hizli buyu -> fade
```

Spark ornegi:

```text
orta -> kucuk -> yok
```

## Noise

Particle hareketine turbulence ve randomness katar.

Smoke, fireflies ve magic dust icin cok onemlidir.

Dikkat:

- Cok yuksek Noise yapay gorunur.
- Cok fazla octave performance maliyeti yaratabilir.

## Texture Sheet Animation

Texture'i grid'e boler.

Ornek 2x2 smoke sheet:

```text
[0] [1]
[2] [3]
```

Kullanim turleri:

- Particle lifetime boyunca frame oynat.
- Her particle random frame secsin.

Smoke icin genelde ikinci kullanim daha dogal olur.

## VFX Graph Rehberi

VFX Graph ne zaman secilir?

- Cok fazla particle gerekiyorsa.
- GPU-based effect istiyorsan.
- Complex simulation yapacaksan.
- URP/HDRP kullaniyorsan.
- Physics dependency dusukse.

VFX Graph ne zaman secilmez?

- Basit pickup effect yapacaksan.
- Mobile/WebGL gibi sinirli platform hedefliyorsan.
- Rigidbody/Collider ile dogrudan interaction gerekiyorsa.
- Yeni basliyorsan ve efekt basitse.

VFX Graph temel akisi:

```text
Spawn -> Initialize -> Update -> Output
```

Spawn:

- Particle sayisini ve timing'i belirler.
- Particle System'deki Emission gibi dusun.

Initialize:

- Particle'in dogum bilgilerini belirler.
- Position, velocity, lifetime, size, color.

Update:

- Particle yasarken ne olacagini belirler.
- Force, turbulence, collision-like behavior, size/color update.

Output:

- Particle'in nasil render edilecegini belirler.
- Texture, material, mesh, billboard, color output.

## Kendi Oyununa Pratik VFX Receteleri

## Pickup Effect

```text
Looping: Off
Play On Awake: Off
Emission: Burst
Shape: Sphere
Color over Lifetime: bright -> transparent
Size over Lifetime: shrink/fade
Material: Additive
```

## Enemy Collision Explosion

```text
Looping: Off
Burst Count: 50-150
Start Speed: Random 2-6
Start Lifetime: Random 0.3-1.2
Shape: Sphere
Color: orange/red/smoke
Optional: Point Light flash + camera shake
```

## Player Dust Trail

```text
Parent: Player
Simulation Space: World
Emission: Rate over Distance
Shape: small Cone/Circle behind player
Color: brown/gray -> transparent
Size: grow then fade
```

## Rain

```text
Shape: Box above camera/player
Start Speed: high
Start Size: small
Renderer: Stretched Billboard
Emission Rate: high
Prewarm: On
```

## Snow

```text
Shape: Box
Start Speed: low
Noise: gentle
Start Lifetime: longer
Prewarm: On
Color: white -> transparent
```

## Fire

```text
Multiple systems:
- Flames: additive, yellow/orange/red, upward
- Smoke: alpha, gray, slow upward
- Sparks: small additive dots, random speed/noise
Optional:
- Point Light with flicker
```

## Performance Kurallari

VFX guzel ama pahali olabilir.

En onemli performans maliyetleri:

| Problem | Neden kotu? | Cozum |
|---|---|---|
| Cok fazla particle | CPU/GPU maliyeti | Rate/Lifetime dusur |
| Cok buyuk transparent particle | Overdraw | Texture boyutunu kucult, alpha alanini azalt |
| Additive/transparent cok overlap | GPU Early-Z optimizasyonu azalir | Daha az yogunluk |
| Max Particles cok yuksek | Gereksiz memory/sim | Gercek ihtiyaca gore ayarla |
| Lights module cok fazla | Cok pahali olabilir | Fake glow / emissive kullan |
| VFX Graph bounds yanlis | Effect culling olabilir | Bounds kontrol et |
| Collision module asiri kullanim | Pahali | Sadece gerekliyse kullan |

Altin formul:

```text
Particle count ~= Emission Rate x Lifetime
```

Yogunlugu artirmadan once su uc seyi dusun:

1. Daha iyi texture kullanabilir miyim?
2. Daha iyi color/size curve ile ayni hissi verebilir miyim?
3. Daha az particle ile daha iyi motion verebilir miyim?

## En Yaygin Hatalar

## Particle gorunmuyor

Kontrol et:

- Play On Awake kapali mi?
- Particle System Play ediliyor mu?
- Emission Rate 0 mi?
- Burst ekledin mi?
- Start Size cok kucuk mu?
- Material transparent mi?
- Alpha 0 mi?
- Camera layer goruyor mu?
- 2D'de Sorting Layer / Order dogru mu?
- Z position camera onunde mi?

## Particle'da bosluklar var

Muhtemel sebep:

```text
Max Particles limitine ulasildi
```

Cozumler:

- Max Particles artir.
- Start Lifetime dusur.
- Start Speed artir.
- Emission Rate dusur.

## Efekt player'a yapisiyor

Muhtemel sebep:

```text
Simulation Space = Local
```

Trail/dust icin cogu zaman:

```text
Simulation Space = World
```

Aura gibi karaktere bagli efekt icin:

```text
Simulation Space = Local
```

## 2D particle arkada veya onde yanlis ciziliyor

Kontrol et:

- Renderer > Sorting Layer
- Renderer > Order in Layer
- Camera projection
- Z position
- Material render queue

## Quiz Icin Ana Mantik

- Particle System bircok kucuk image veya mesh render ederek effect olusturur.
- Start Lifetime, bir particle'in kac saniye yasayacagini belirler.
- Shape module, emitter'in seklini belirler; particle'in kendi seklini degil.
- Emission bosluklari genelde Max Particles limitinden olur.
- One-shot effect icin Looping kapatilir; gercek burst icin Bursts kullanilir.
- Edge yerine Volume'dan emit etmek icin Shape > Emit From ayari degistirilir.
- Gercek module'lar: Sub Emitters, Velocity over Lifetime, Size over Lifetime, Color over Lifetime.
- Color over Lifetime, particle yaslandikca renk/alpha degisimini belirler.
- 2D optimization icin Main module'daki Gravity Source degerini 2D Physics yapabilirsin.

## Onerilen Calisma Sirasi

1. Hazir fire prefab'ini ac, child particle system'leri incele.
2. Sadece Sparks uzerinde Emission, Color over Lifetime ve Noise ayarlarini degistir.
3. Sifirdan FX_Snow veya FX_Rain yap.
4. Sifirdan FX_SmokeBurst yap.
5. SmokeBurst'u Space tusu veya collision event ile tetikle.
6. Magical challenge effect yap. Fireflies iyi baslangictir.
7. Kendi oyununda once pickup effect ekle.
8. Sonra enemy collision/death effect ekle.
9. En son polish ekle: light flash, sound, camera shake.

## Kaynaklar

- Unity Learn Game Development Pathway - Visual Effects Unit, Unity 6.3
- Unity Manual 6.3 - Particle effects
- Unity Manual 6.3 - Choosing your particle system solution
- Unity Manual 6.3 - Particle System modules
- Unity Manual 6.3 - Main, Emission, Shape, Renderer, Color over Lifetime, Size over Lifetime, Noise, Texture Sheet Animation modules
- Unity VFX Graph 17.6 docs - Getting Started, Graph Logic, Requirements and Compatibility
- Unity URP Particles Unlit shader docs
- Unity ShaderLab Blend docs
