namespace fmedge.Model
{
    /// <summary>
    /// KEPS 모듈의 데이터 수신 체크 메세지 포맷 클래스
    /// </summary>
    public class ComHttpPacket
    {
        public string building_id { get; set; }
        public string inspection_result_cd { get; set; }
        public string inspection_result_val { get; set; }
        public string inspection_datetime { get; set; }
    }
}
