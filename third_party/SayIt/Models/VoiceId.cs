namespace SayIt
{
    public sealed record VoiceId
    {
        public string Name { get; }

        private VoiceId(string name) => Name = name;

        public override string ToString() => Name;

        // ── English ──────────────────────────────────────────────
        public static readonly VoiceId EnUSJennyNeural  = new("en-US-JennyNeural");
        public static readonly VoiceId EnUSAriaNeural   = new("en-US-AriaNeural");
        public static readonly VoiceId EnUSGuyNeural    = new("en-US-GuyNeural");
        public static readonly VoiceId EnUSDavisNeural  = new("en-US-DavisNeural");
        public static readonly VoiceId EnUSJaneNeural   = new("en-US-JaneNeural");
        public static readonly VoiceId EnUSNancyNeural  = new("en-US-NancyNeural");
        public static readonly VoiceId EnUSSaraNeural   = new("en-US-SaraNeural");
        public static readonly VoiceId EnUSTonyNeural   = new("en-US-TonyNeural");

        public static readonly VoiceId EnGBSoniaNeural  = new("en-GB-SoniaNeural");
        public static readonly VoiceId EnGBRyanNeural   = new("en-GB-RyanNeural");
        public static readonly VoiceId EnGBLibbyNeural  = new("en-GB-LibbyNeural");

        public static readonly VoiceId EnAUNatashaNeural = new("en-AU-NatashaNeural");
        public static readonly VoiceId EnAUWilliamNeural = new("en-AU-WilliamNeural");

        public static readonly VoiceId EnCAClaraNeural  = new("en-CA-ClaraNeural");
        public static readonly VoiceId EnCALiamNeural   = new("en-CA-LiamNeural");

        public static readonly VoiceId EnINNeerjaNeural = new("en-IN-NeerjaNeural");
        public static readonly VoiceId EnINPrabhatNeural = new("en-IN-PrabhatNeural");

        // ── Chinese ─────────────────────────────────────────────
        public static readonly VoiceId ZhCNXiaoxiaoNeural  = new("zh-CN-XiaoxiaoNeural");
        public static readonly VoiceId ZhCNYunyangNeural   = new("zh-CN-YunyangNeural");
        public static readonly VoiceId ZhCNXiaochenNeural  = new("zh-CN-XiaochenNeural");
        public static readonly VoiceId ZhCNXiaohanNeural   = new("zh-CN-XiaohanNeural");
        public static readonly VoiceId ZhCNXiaomengNeural  = new("zh-CN-XiaomengNeural");
        public static readonly VoiceId ZhCNXiaomoNeural    = new("zh-CN-XiaomoNeural");
        public static readonly VoiceId ZhCNXiaoruiNeural   = new("zh-CN-XiaoruiNeural");
        public static readonly VoiceId ZhCNXiaoshuangNeural = new("zh-CN-XiaoshuangNeural");
        public static readonly VoiceId ZhCNXiaoyouNeural   = new("zh-CN-XiaoyouNeural");
        public static readonly VoiceId ZhCNYunxiNeural     = new("zh-CN-YunxiNeural");
        public static readonly VoiceId ZhCNYunyeNeural     = new("zh-CN-YunyeNeural");

        public static readonly VoiceId ZhTWHsiaoChenNeural = new("zh-TW-HsiaoChenNeural");
        public static readonly VoiceId ZhTWHsiaoYuNeural   = new("zh-TW-HsiaoYuNeural");
        public static readonly VoiceId ZhTWYunJheNeural    = new("zh-TW-YunJheNeural");

        public static readonly VoiceId ZhHKHiuGaaiNeural   = new("zh-HK-HiuGaaiNeural");
        public static readonly VoiceId ZhHKHiuMaanNeural   = new("zh-HK-HiuMaanNeural");
        public static readonly VoiceId ZhHKWanLungNeural   = new("zh-HK-WanLungNeural");

        // ── Japanese ────────────────────────────────────────────
        public static readonly VoiceId JaJPNanamiNeural = new("ja-JP-NanamiNeural");
        public static readonly VoiceId JaJPKeitaNeural  = new("ja-JP-KeitaNeural");

        // ── Korean ──────────────────────────────────────────────
        public static readonly VoiceId KoKRSunHiNeural  = new("ko-KR-SunHiNeural");
        public static readonly VoiceId KoKRInJoonNeural = new("ko-KR-InJoonNeural");

        // ── French ──────────────────────────────────────────────
        public static readonly VoiceId FrFRDeniseNeural   = new("fr-FR-DeniseNeural");
        public static readonly VoiceId FrFRHenriNeural    = new("fr-FR-HenriNeural");
        public static readonly VoiceId FrFRVivienneNeural = new("fr-FR-VivienneNeural");

        public static readonly VoiceId FrCASylvieNeural   = new("fr-CA-SylvieNeural");
        public static readonly VoiceId FrCAAntoineNeural  = new("fr-CA-AntoineNeural");

        public static readonly VoiceId FrCHArianeNeural   = new("fr-CH-ArianeNeural");

        // ── German ──────────────────────────────────────────────
        public static readonly VoiceId DeDEKatjaNeural   = new("de-DE-KatjaNeural");
        public static readonly VoiceId DeDEConradNeural  = new("de-DE-ConradNeural");
        public static readonly VoiceId DeDEAmalaNeural   = new("de-DE-AmalaNeural");
        public static readonly VoiceId DeDEBerndNeural   = new("de-DE-BerndNeural");

        public static readonly VoiceId DeATIngridNeural  = new("de-AT-IngridNeural");
        public static readonly VoiceId DeATJonasNeural   = new("de-AT-JonasNeural");

        // ── Spanish ─────────────────────────────────────────────
        public static readonly VoiceId EsESAlvaroNeural  = new("es-ES-AlvaroNeural");
        public static readonly VoiceId EsESElviraNeural  = new("es-ES-ElviraNeural");

        public static readonly VoiceId EsMXJorgeNeural   = new("es-MX-JorgeNeural");
        public static readonly VoiceId EsMXDaliaNeural   = new("es-MX-DaliaNeural");

        public static readonly VoiceId EsARElenaNeural   = new("es-AR-ElenaNeural");
        public static readonly VoiceId EsARTomasNeural   = new("es-AR-TomasNeural");

        // ── Italian ─────────────────────────────────────────────
        public static readonly VoiceId ItITElsaNeural     = new("it-IT-ElsaNeural");
        public static readonly VoiceId ItITIsabellaNeural = new("it-IT-IsabellaNeural");
        public static readonly VoiceId ItITDiegoNeural    = new("it-IT-DiegoNeural");

        // ── Portuguese ──────────────────────────────────────────
        public static readonly VoiceId PtBRFranciscaNeural = new("pt-BR-FranciscaNeural");
        public static readonly VoiceId PtBRAntonioNeural   = new("pt-BR-AntonioNeural");

        public static readonly VoiceId PtPTFernandaNeural  = new("pt-PT-FernandaNeural");
        public static readonly VoiceId PtPTDuarteNeural    = new("pt-PT-DuarteNeural");

        // ── Russian ─────────────────────────────────────────────
        public static readonly VoiceId RuRUSvetlanaNeural = new("ru-RU-SvetlanaNeural");
        public static readonly VoiceId RuRUDariyaNeural   = new("ru-RU-DariyaNeural");
        public static readonly VoiceId RuRUDmitryNeural   = new("ru-RU-DmitryNeural");

        // ── Arabic ──────────────────────────────────────────────
        public static readonly VoiceId ArSAZariyahNeural  = new("ar-SA-ZariyahNeural");
        public static readonly VoiceId ArSAHamedNeural    = new("ar-SA-HamedNeural");
        public static readonly VoiceId ArEGSalmaNeural    = new("ar-EG-SalmaNeural");
        public static readonly VoiceId ArEGShakirNeural   = new("ar-EG-ShakirNeural");

        // ── Dutch ───────────────────────────────────────────────
        public static readonly VoiceId NlNLFennaNeural    = new("nl-NL-FennaNeural");
        public static readonly VoiceId NlNLMaartenNeural  = new("nl-NL-MaartenNeural");

        // ── Polish ──────────────────────────────────────────────
        public static readonly VoiceId PlPLAgnieszkaNeural = new("pl-PL-AgnieszkaNeural");
        public static readonly VoiceId PlPLMarekNeural     = new("pl-PL-MarekNeural");

        // ── Turkish ─────────────────────────────────────────────
        public static readonly VoiceId TrTREmelNeural     = new("tr-TR-EmelNeural");
        public static readonly VoiceId TrTRAhmetNeural    = new("tr-TR-AhmetNeural");

        // ── Swedish ─────────────────────────────────────────────
        public static readonly VoiceId SvSESofieNeural    = new("sv-SE-SofieNeural");
        public static readonly VoiceId SvSEMattiasNeural  = new("sv-SE-MattiasNeural");

        // ── Norwegian ───────────────────────────────────────────
        public static readonly VoiceId NbNOPernilleNeural = new("nb-NO-PernilleNeural");
        public static readonly VoiceId NbNOFinnNeural     = new("nb-NO-FinnNeural");

        // ── Danish ──────────────────────────────────────────────
        public static readonly VoiceId DaDKChristelNeural = new("da-DK-ChristelNeural");
        public static readonly VoiceId DaDKJeppeNeural    = new("da-DK-JeppeNeural");

        // ── Finnish ─────────────────────────────────────────────
        public static readonly VoiceId FiFISelmaNeural    = new("fi-FI-SelmaNeural");
        public static readonly VoiceId FiFIHarriNeural    = new("fi-FI-HarriNeural");

        // ── Hindi ───────────────────────────────────────────────
        public static readonly VoiceId HiINSwaraNeural    = new("hi-IN-SwaraNeural");
        public static readonly VoiceId HiINMadhurNeural   = new("hi-IN-MadhurNeural");

        // ── Thai ────────────────────────────────────────────────
        public static readonly VoiceId ThTHPremwadeeNeural = new("th-TH-PremwadeeNeural");
        public static readonly VoiceId ThTHNiwatNeural     = new("th-TH-NiwatNeural");

        // ── Vietnamese ──────────────────────────────────────────
        public static readonly VoiceId ViVNHoaiMyNeural   = new("vi-VN-HoaiMyNeural");
        public static readonly VoiceId ViVNNamMinhNeural  = new("vi-VN-NamMinhNeural");

        // ── Indonesian ──────────────────────────────────────────
        public static readonly VoiceId IdIDGadisNeural    = new("id-ID-GadisNeural");
        public static readonly VoiceId IdIDArdiNeural     = new("id-ID-ArdiNeural");
    }
}
