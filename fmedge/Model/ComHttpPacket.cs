namespace fmedge.Model
{
    /// <summary>
    /// KEPS 모듈의 데이터 수신 체크 메세지 포맷 클래스
    /// </summary>
    class ComHttpPacket
    {
        public string building_id;
        public string inspection_result_cd;
        public string inspection_result_val;
        public string inspection_datetime;
    }
}
