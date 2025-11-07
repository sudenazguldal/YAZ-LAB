## Oyunun Özellikleri
Bu proje, TPS türünün temel mekaniklerini barındıran, yapay zekâ destekli düşman karakterler (NPC) içeren bir oyun geliştirmeyi amaçlamaktadır. 
Geliştirilen oyun, oyuncuya temel TPS deneyimini sunarken; kamera, hareket, animasyon, ateş etme ve siper alma sistemleriyle birlikte eksiksiz bir oynanış akışı sağlamaktadır. Oyun PC platformu için geliştirilmiş olup, Unity oyun motoru kullanılarak inşa edilmiştir.
Grafikler low-poly tarzında seçilmiş ve performans odaklı bir yapı benimsenmiştir.

### Oyuncu (Player) Mekanikleri
Oyuncu karakteri, Third Person Controller altyapısına sahiptir. Hareket sistemi WASD tuşlarıyla yürüyüş ve koşma kontrolünü sağlar. C tuşu toggle ile, Left Ctrl tuşu ise basılı tutularak geçici siper alma/dinamik eğilme gerçekleştirilebilir. Kamera sistemi, Third Person ve Aim Camera arasında geçiş yapabilen  dinamik bir Camera Switcher yapısına sahiptir.
Ateş etme (shooting) mekanikleri aktif olup, nişan alma sırasında kamera dinamik olarak yakınlaşır. Avatar Mask ve Blend Tree yapılarını kullanan gelişmiş animasyon sistemi sayesinde karakterin yürüme, nişan alma, crouch,siper alma ve ateş etme hareketleri arasında akıcı geçişler sağlanmıştır.

### Düşman (NPC) Yapay Zekâsı
Düşman karakterleri, NavMesh Agent tabanlı bir navigasyon sistemi ile oyuncuyu tespit eder ve takip eder. Düşman davranışları; Idle, Patrol, Walk, Attack ve Death durumları arasında geçiş yapan bir Finite State Machine (FSM) yapısıyla kontrol edilmektedir.
Enemy Controller script’i ve Animator parametreleri ile entegre çalışan bu sistem, düşmanların oyuncuya tepki vermesini, belirli alanlarda devriye gezmesini ve saldırı gerçekleştirmesini sağlar. Ayrıca düşmanlar, belirli bölgelerde Spawner sistemi aracılığıyla dinamik olarak oluşturulmaktadır.
Düşman davranışları; Idle, Patrol, Chase, Attack ve Walk olmak üzere farklı durumlara (state) ayrılmıştır.

### Kullanıcı Arayüzü (UI)
Kullanıcı arayüzü, oyuncuya oyun içi bilgileri sade ve okunabilir biçimde sunacak şekilde tasarlanmıştır.
+ Can Barı (Health Bar): Oyuncunun sağlık durumunu gösterir.
+ Cephanelik Göstergesi (Ammo Display): Şarjörde kalan mermi miktarını gösterir.
+ Pause ve Main Menu: Oyunun durdurulması, devam ettirilmesi veya ana menüye dönülmesini sağlar.
+ Settings Menüsü: Ses, grafik ve kontrol ayarlarının düzenlenmesine olanak tanır.
+ Kayıt Sistemi (Save/Load): Oyunun ilerleyişi kaydedilebilir ve yeniden yüklenebilir.
+ Eşya Toplama Sistemi: Oyuncu belirli nesneleri toplayabilir, envanterinde saklayabilir ve kullanabilir.
+ Crosshair Seçimi: Oyuncu nişangah tasarımını kendi tercihlerine göre değiştirebilir.

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

Diyalog bitince “Delirme Anı (Delirium)” başlar: ekran titreşir, sesler boğulur, kalp atışı sesi duyulur. Oyuncu, Doktor’a nişan almak zorundadır.

### 4. Çözüm (Resolution) – Son Atış

 Oyuncu ateş ettiğinde ve Doktor’un canı sıfıra indiğinde: Doktor ölür, FinalConfrontation tüm sesleri ve efektleri durdurur, Zaman durur (Time.timeScale = 0f) ve oyun sona erer.

##  Sistem Blok Diyagramı

Aşağıda *Project: Macula* oyununda kullanılan temel sistem bileşenleri ve aralarındaki ilişki gösterilmektedir:

##  Karşılaşılan Zorluklar ve Çözümler
* Enemy'ler oyuncuyu sürekli takip edecek şekilde ayarlandığından, saldırı durumundayken oyuncu hareket ettiğinde kayarak takip ediyorlardı. İki gün süren uğraşlarım sonucunda, sorunun animatördeki “Has Exit Time” seçeneğini kapatmamla düzeldiğini fark ettim.
* Projemiz düz bir zeminden oluşmadığı için, NavMesh Surface yere yerleştirilen küçük objeleri de dahil ediyordu. Bu durum, düşmanların bazen havada durmasına veya bazı bölgelerde geçişleri engel olarak algılayıp NavMesh yüzeyinin birleşmemesine neden oluyordu. İlk etapta tek tek duvarları ve yerdeki küçük eşyaları kaldırarak sorunu çözmeye çalıştım; ancak asıl çözümün, AI Navigation ayarlarını küçülterek sağlandığını fark ettim.
* Enemy’lerin animasyonlarında zaman zaman beklenmeyen hatalar oluşuyor. Bazen animasyonlar düzgün şekilde çalışırken, bazen de hareketler yanlış bir biçimde tekrar ediyor. Bu sorunun nedenini tam olarak tespit edemedim.


