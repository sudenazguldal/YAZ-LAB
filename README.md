## Oyunun Özellikleri
Bu proje, TPS türünün temel mekaniklerini barındıran, yapay zekâ destekli düşman karakterler (NPC) içeren bir oyun geliştirmeyi amaçlamaktadır. 
Geliştirilen oyun, oyuncuya temel TPS deneyimini sunarken; kamera, hareket, animasyon, ateş etme ve siper alma sistemleriyle birlikte eksiksiz bir oynanış akışı sağlamaktadır. Oyun PC platformu için geliştirilmiş olup, Unity oyun motoru kullanılarak inşa edilmiştir.
Grafikler low-poly tarzında seçilmiş ve performans odaklı bir yapı benimsenmiştir.

### Oyuncu (Player) Mekanikleri
Oyuncu karakteri, Third Person Controller altyapısına sahiptir. 
+ Hareket sistemi WASD tuşlarıyla yürüyüş ve koşma kontrolünü sağlar, Animator Controller içerisinde olan layerlar ve rigler ile gerçekçi bir hissiyat oluşturur.
+ C tuşu toggle ile, Left Ctrl tuşu ise basılı tutularak geçici siper alma/crouch gerçekleştirilebilir. Siper alma ve Crouch birbirine bağlıdır. Karakterin önünde cover alabilceği bir nesne varsa cover alır yoksa croucha geçer. Eğer cover sırasında cover aldığı nesneden uzaklaşırsa otomatik olarak crouch pozisyonuna döner. Visibiltysi bu durum geçişlerine göre değişir.
+ Kamera sistemi, Third Person ve Aim Camera arasında geçiş yapabilen  dinamik bir Camera Switcher yapısına sahiptir.
+ Ateş etme (shooting) mekanikleri aktif olup, nişan alma sırasında kamera dinamik olarak yakınlaşır ve over the shoulder hissiyatı sağlanır. Eğer karakter eğim durumda değilse(Camera da AimCam de) karakter ateş edemez. 
+ Avatar Mask ve Blend Tree yapılarını kullanan gelişmiş animasyon sistemi sayesinde karakterin yürüme, nişan alma, crouch,siper alma ve ateş etme hareketleri arasında akıcı geçişler sağlanmıştır.

### Düşman (NPC) Yapay Zekâsı
Düşman karakterleri, NavMesh Agent tabanlı bir navigasyon sistemi ile oyuncuyu tespit eder ve takip eder. Düşman davranışları; Idle, Patrol, Walk, Attack ve Death durumları arasında geçiş yapan bir Finite State Machine (FSM) yapısıyla kontrol edilmektedir.
Enemy Controller script’i ve Animator parametreleri ile entegre çalışan bu sistem, düşmanların oyuncuya tepki vermesini, belirli alanlarda devriye gezmesini ve saldırı gerçekleştirmesini sağlar. Ayrıca düşmanlar, belirli bölgelerde Spawner sistemi aracılığıyla dinamik olarak oluşturulmaktadır.
Düşman davranışları; Idle, Patrol, Chase, Attack ve Walk olmak üzere farklı durumlara (state) ayrılmıştır.

### Kullanıcı Arayüzü (UI)
Kullanıcı arayüzü, oyuncuya oyun içi bilgileri sade ve okunabilir biçimde sunacak şekilde tasarlanmıştır.
+ Can Barı (Health Bar): Oyuncunun sağlık durumunu gösterir.
+ Cephanelik Göstergesi (Ammo Display): Şarjörde kalan mermi miktarını gösterir.
+ Pause Menu: Oyunun durdurulması, devam ettirilmesi, baştan başlaması, kaydedilmesi veya ana menüye dönülmesini sağlar.
+ Main Menu: Oyunun açılış ekranıdır; oyuna kaldığı yerden devam etmesini, baştan başlamasını, ayarlar menüsüne yönlendirilmessini veya oyundan çıkmasını sağlar.
+ Settings Menüsü: Ses, grafik ve kontrol ayarlarının düzenlenmesine olanak tanır.
+ Kayıt Sistemi (Save/Load): Oyunun ilerleyişi kaydedilebilir ve yeniden yüklenebilir.
+ Eşya Toplama Sistemi: Oyuncu belirli nesneleri toplayabilir, saklayabilir ve kullanabilir.
+ Crosshair Seçimi: Oyuncu nişangah tasarımını kendi tercihlerine göre değiştirebilir.
+ Notification sistemi ile toplanan malzemeyi bildirim olarak gösterir.
+ Objective ile oyuncunun yapması gereken görevleri gösterir ve oyuncunun hikayede kaybolmasını engeller.


## Geliştirme Ortamı
+ Oyun Motoru: Unity 6000.0.58f1 LTS
+ Programlama Dili: C#
+ IDE: Visual Studio 2022
+ Versiyon Kontrol: Git & GitHub (proje iş birliği ve yedekleme amaçlı)
+ Hedef Platform: Windows (PC)
+ Render Pipeline: Unity URP (Universal Render Pipeline)

## Kullanılan Eklentiler ve Teknolojiler

Projede geliştirilen mekaniklerin yanında, oyun deneyimini desteklemek için çeşitli Unity bileşenleri, sistemleri ve eklentilerden yararlanılmıştır:

| **Kategori**           | **Kullanılan Sistem / Araç**              | **Açıklama**                                                   |
| ---------------------- | ----------------------------------------- | -------------------------------------------------------------- |
| **Karakter Kontrolü**  | Character Controller, Cinemachine         | TPS ve Aim kamera geçişi, kamera takibi                        |
| **Yapay Zekâ (AI)**    | NavMesh Agent, FSM State Scripts          | Düşmanların devriye, takip ve saldırı davranışları             |
| **Animasyon**          | Animator, Blend Tree, Avatar Mask         | Yürüme, nişan alma, cover ve atış animasyon geçişleri          |
| **Ses Sistemi**        | Audio Mixer, Audio Source                 | Arka plan müziği, efekt sesleri, menü sesleri                  |
| **UI Sistemi**         | Unity UI Toolkit (Canvas, Slider, Button) | Can barı, cephane göstergesi, menüler ve ayarlar               |
| **Kayıt Sistemi**      | Esper Save System                         | Oyuncu konumu, sağlık ve cephane verilerinin kaydedilmesi      |
| **Sürüm Takibi**       | GitHub Desktop                            | Ekip üyeleri arasında kod senkronizasyonu ve versiyon kontrolü |



## Senaryo 
Oyun Adı: Project: Macula
Tür: Üçüncü Şahıs (TPS) – Hayatta Kalma / Gerilim

### 1. Giriş (Prologue)

Oyun, Polis Memuru karakterinin, terk edilmiş ve lanetli olarak bilinen “Horror Mansion” bölgesine tekrar gönderilmesiyle başlar. Merkez, konaktan gelen “asılsız çığlık” ihbarlarını araştırmaktadır.
Ancak karakterin bu eve gelişi tesadüf değildir. Oyuncu, Doktor’un deneylerinden “o gece” kaçmayı başaran tek “kusurlu” denektir. Gelen ihbarlar aslında Doktor’un, kaçan deneği (oyuncuyu) geri çağırma yöntemidir.

### 2. Yükseliş (Rising Action)

Evle Yüzleşme: Oyuncu konağa yaklaştıkça geçmiş travmaları tetiklenir.
Diyalog: “Yine mi bu ev?... Biliyorum, bu lanet yerin benimle işi daha bitmedi.”

Sesin Keşfi: Evin içinden gelen ağlama seslerinin “asılsız” değil, gerçek olduğu fark edilir.
Diyalog: “Kahretsin... İşte o ses. Demek ihbarlar... bu kez gerçekti.”

Tuzak: Oyuncu ana kapıyı açtığında Doktor’un tuzağı tetiklenir; zombiler serbest kalır, müzik gerilime dönüşür.
Görev: “Kütüphaneye ulaş.”
Diyalog: “Yine başlıyor... Olamaz, yine o geceki gibi! Kütüphaneye ulaşmam lazım... Yan kapıdan!”

Kütüphane kapısına ulaşan oyuncu kapının kilitli olduğunu görür.
Diyalog: “Kilitli... Tabii ki kilitli. Biliyordum... Beni yine buraya çekeceğini biliyordum. 'O gece' yarım kalan hesabı bitirmenin vakti geldi, Doktor.”
Görev Güncellemesi: “Anahtarı bul.”


### 3. Zirve (Climax) – Yüzleşme
Oyuncu anahtarı bulur ve Kütüphane kapısını açar. Bu anda FinalConfrontation tetiklenir:

Oyuncu kontrolü devre dışı bırakılır, kamera sabitlenir.

Diyalog başlar:
Doktor: “İnanılmaz. Bütün kusursuz yaratıklarımın arasından sıyrılıp yine geldin. Sen bu deneyin en inatçı çarpıklığısın.”
Oyuncu: “Deney bitti, Doktor. Yarım kalan ne varsa bu gece sona erecek.”

Diyalog bitince “Delirme Anı (Delirium)” başlar: ekran titreşir, sesler boğulur, ağlama sesi duyulur. Oyuncu, Doktor’a nişan almak zorundadır.

### 4. Çözüm (Resolution) – Son Atış

 Oyuncu ateş ettiğinde ve Doktor’un canı sıfıra indiğinde: Doktor ölür, FinalConfrontation tüm sesleri ve efektleri durdurur, Zaman durur (Time.timeScale = 0f) ve oyun sona erer.

##  Sistem Blok Diyagramı

Aşağıda *Project: Macula* oyununda kullanılan temel sistem bileşenleri ve aralarındaki ilişki gösterilmektedir:
![Blok Diyagramı](https://github.com/sudenazguldal/YAZ-LAB/commit/3e734042b426cecf38f33202d616103f321cbb3c)


##  Karşılaşılan Zorluklar ve Çözümler
* Enemy'ler oyuncuyu sürekli takip edecek şekilde ayarlandığından, saldırı durumundayken oyuncu hareket ettiğinde kayarak takip ediyorlardı. İki gün süren uğraşlarım sonucunda, sorunun animatördeki “Has Exit Time” seçeneğini kapatmamla düzeldiğini fark ettim.
* Projemiz düz bir zeminden oluşmadığı için, NavMesh Surface yere yerleştirilen küçük objeleri de dahil ediyordu. Bu durum, düşmanların bazen havada durmasına veya bazı bölgelerde geçişleri engel olarak algılayıp NavMesh yüzeyinin birleşmemesine neden oluyordu. İlk etapta tek tek duvarları ve yerdeki küçük eşyaları kaldırarak sorunu çözmeye çalıştım bu çözümün sonucunda haritanın anlamsız yerlerinde ev eşyaları bulmamızla sonladı (çalılıkların arasında uçan kitaplar vb.);  asıl çözümün, AI Navigation ayarlarını küçülterek sağlandığını fark ettim.
* Enemy’lerin animasyonlarında zaman zaman beklenmeyen hatalar oluşuyor. Bazen animasyonlar düzgün şekilde çalışırken, bazen de hareketler yanlış bir biçimde tekrar ediyor. Bu sorunun nedenini tam olarak tespit edemedim.
* Karakter crouch pozisyonuna girip haraket edince sol elı anlamsız bir şekilde havaya kaldırıp görüntünün gerçekçiliğini bozuyordu, avatar mask kullanarak sadece sol eli etkileyen bir layer oluşturup çözdüm.
* Bulduğum animasyonlarla karakterlerin root sisteminin isimlendirmesi uyuşmuyordu. Blender kullanarak kemikleri tekrar isimlendirdim. 2 günlük bu emeğin sonunda başka karakter kullanmaya karar verip olası yeni sounları çözdüm.
+ Pause menü tasarımı için harika bi asset bulmuştum ama unity ile uyumlu olmadığı için paneli kendim yapmak zorunda kaldım. Buna rağmen çok temiz aktı. Sonrasında main menüyü'de pause menüden alırım dedim. Ama sebebini anlamadığım bir şekilde main menüdeki butonlar çalışmadı. Saatlerce uğraşmalarımın sonucu boşa çıkınca paneli komple sildim. Tekrar yaptım ve tabii ki yine çalışmadı. Derin hezeyanlarımdan sonra tekrar silip yaptım ve ne fark ettim biliyor musunuz? panele vertical group layer eklediğimden dolayı yüksekliği sabitliyor ve nedendir bilinmez 0'a sabitliyormuş. Yüksekliği 0 olan butonların neden çalışmadığını anlamış oldum. Ama bu durumu peşimi daha sonrasında da bırakmadı.Butonlar çalışmıyorsa projenin input sistemi hatalı herhalde diyerekten input sistemini değiştirmişim sonra butonlar çalışıncada sistemi tekrardan değiştirmeyi unutma gafletinde bulunmuşum. 1 gün sonra paneli kontrol edeyim diye editörü çalıştırayım dedim, bizim başkarakter zemine sen gömül ve çıkma. Bi 3,4 saatte onunla uğraştım. Daha da akıllandım input sistemi ile oynamıyorum. Bir de group layer kullanmıyorum tabii.
+ Settings kısmına nişan hassasiyetini ekledikten sonra bi' heves karakter hassasiyeti de ekleyeyim dedim. Karakter tüm dönebilme kabiliyetini kaybedince ve yarışçı arazi arabaları gibi bi o yana bi bu yana savrulunca bu hevesten vazgeçtim.
+ Settings kısımındaki ses sliderleri için başta aralık olarak -80dB 0dB aldım. Fakat ses henüz %75'teyken 0'landığı için desibeli -20'ye çekmeye karar verdim. İyi bir karardı.
* ### MERGE
## Literatür Taraması 
| **Özellik**        | **Projemiz (Project: Macula)**                     | **Resident Evil 2 (Remake)**            | **Silent Hill 2 (Remake)**        | **The Evil Within**            | **The Last of Us (Part I)**   |
| ------------------ | -------------------------------------------------- | --------------------------------------- | --------------------------------- | ------------------------------ | ----------------------------- |
| **Oyun Motoru**    | Unity 6                                            | RE Engine                               | Unreal Engine 5                   | id Tech 5 (Modifiye)           | Naughty Dog Engine            |
| **Ana Tema**       | Psikolojik Gerilim (Deney, Travma)                 | Hayatta Kalma Korkusu (Biyolojik Silah) | Psikolojik Korku (Kişisel Travma) | Psikolojik Korku (Görsel/Gore) | Hayatta Kalma (Anlatı Odaklı) |
| **Kamera**         | TPS (Omuz Üstü Aim)                                | TPS (Omuz Üstü Aim)                     | TPS (Omuz Üstü)                   | TPS (Omuz Üstü)                | TPS (Omuz Üstü)               |
| **AI Sistemi**     | FSM + NavMesh                                      | Gelişmiş FSM / Davranışsal              | FSM                               | FSM + Behavior Tree            | Gelişmiş FSM / Davranışsal    |
| **Pathfinding**    | NavMesh                                            | NavMesh (Özel)                          | NavMesh                           | NavMesh                        | NavMesh                       |
| **Harita Yapısı**  | Kapalı Mekan (Korku Konağı)                        | Kapalı Mekan (Lineer İlerleme)          | Yarı-Açık Dünya (Kasaba)          | Bölüm Bazlı (Lineer)           | Geniş Lineer (Wide–Linear)    |
| **Düşman Türleri** | Çoklu Türler (Warden, Gorgon, Doktor vb.)          | Çoklu Türler                            | Çoklu Türler                      | Çoklu Türler                   | Çoklu Türler                  |
| **Ses Sistemi**    | Dinamik 3D + 2D Müzik *(Atmosfer ve Müzik Geçişi)* | 3D Çevresel (Binaural)                  | 3D Çevresel                       | 3D Çevresel                    | 3D Çevresel                   |

## Projemizin Farklılıkları ve Katkıları 

### 1. Basitleştirilmiş ve Anlaşılır AI Sistemi
Literatürde yer alan karmaşık Behavior Tree (Davranış Ağacı) sistemleri yerine, anlaşılması ve yönetilmesi daha kolay olan FSM (Finite State Machine – Sonlu Durum Makinesi) yaklaşımı kullanılmıştır.
Bu yöntem, özellikle Unity öğrenen öğrenciler ve yeni başlayan geliştiriciler için daha erişilebilir ve öğretici bir yapı sunmaktadır.

🔹  Resident Evil 2 Remake ve The Last of Us gibi AAA oyunlarda karmaşık davranış ağaçları kullanılırken, projemiz öğretici amaçla sadeleştirilmiş bir FSM sistemini tercih etmiştir.

### 2. Eğitim Odaklı ve Modüler Yapı
Her sistem (PlayerController, PlayerShooting, InventoryCollector, UIManager vb.) kendi sorumluluğunu bilen bağımsız component (bileşen) olarak tasarlanmıştır.
Bu yapı, Component-Based Architecture prensiplerini doğrudan göstermekte ve gelecekte yapılacak yeni mekanik eklemeleri için kolaylık sağlamaktadır.

🔹  Bu sayede oyun, Silent Hill 2 Remake gibi büyük ölçekli yapılara benzer bir organizasyon modelini eğitim ortamına uygun şekilde örneklemektedir.

### 3. Esnek Kontrol Mekanikleri
Projemiz, oyuncu konforunu (Quality of Life) ön planda tutarak, siper alma mekaniği için hem
* Toggle (Bas-Çek – C tuşu)
* Hold (Basılı Tut – Ctrl tuşu)
seçeneklerini aynı anda destekler.
Bu esneklik, türün birçok örneğinde bulunmayan bir kullanıcı deneyimi sağlamaktadır.

🔹 Bu özellik, The Last of Us ve Resident Evil 2 Remake gibi modern TPS oyunlarında dahi nadir görülen bir çift kontrol seçeneği sunar.

### 4. Settings Ayarları 
Modern oyunlarda “Settings” menüsü yalnızca ses ve grafik ayarlarını değil, erişilebilirlik ve kişiselleştirme seçeneklerini de içerir.
Projemizde:
* Ses Kontrolleri: Master, Müzik, SFX ayrı ayrı ayarlanabilir.
* Görüntü Kalitesi: Kalite seviyeleri ve tam ekran seçenekleri.
* Kontrol Hassasiyeti: Nişangah (crosshair) tipi seçimi ve aim hassasiyeti ayarları.

🔹 Bu yapı, Resident Evil 2 Remake gibi AAA oyunların menü sistemlerinden ilham alarak sadeleştirilmiş bir versiyon olarak tasarlanmıştır.

### 5. Sık Tekrar Eden Nesneler
 Oyun geliştirme literatüründe, sık tekrar eden nesnelerin (örneğin düşmanlar veya mermiler) sürekli oluşturulup silinmesi performans kaybına yol açabilir.
Bu nedenle Object Pooling (Nesne Havuzu) yöntemi önerilmektedir.
Projemizde bu sistem henüz uygulanmamıştır; ancak ilerleyen aşamalarda havuz mantığı ile respawn işlemleri daha verimli hale getirilebilir.

🔹 Bu, The Evil Within gibi çok düşman içeren sahnelerde kullanılan optimizasyon tekniklerinin sadeleştirilmiş versiyonudur.

### 6. Harita ve Atmosfer Tasarımı
Proje, Resident Evil 2 ve Silent Hill 2’deki gibi kapalı, baskılayıcı mekân hissi yaratmayı amaçlamaktadır.
Yalnızca “Korku Konağı” gibi küçük ama detaylı bir alan tasarımıyla, narratif yoğunluk (hikaye odaklı deneyim) ön plana çıkarılmıştır.

🔹 Küçük alan + yüksek detay, performansı artırırken atmosfer derinliğini korur.

### 7. Ses ve Müzik Sistemi
Projemiz, 2D arka plan müziği ile 3D çevresel ses sistemini birleştirir.
Oyun içi müzik geçişleri, oyuncunun bulunduğu alana ve duruma göre dinamik olarak değişmektedir.

🔹 Bu yaklaşım, AAA oyunlardaki karmaşık “adaptive audio” sisteminin basitleştirilmiş bir eğitim versiyonudur.
