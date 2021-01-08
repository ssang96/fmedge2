namespace fmedge.Model
{
    /// <summary>
    /// 빌딩과 KEP 서버간의 통신 상태 
    /// </summary>
    public class EventHealth
    {
        public string type { get; set; }
        public string building_id { get; set; }
        public string inspection_result_cd { get; set; }
        public string inspection_result_val { get; set; }
        public string inspection_datetime { get; set; }
    }
}
