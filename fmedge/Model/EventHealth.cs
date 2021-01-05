namespace fmedge.Model
{
    /// <summary>
    /// 빌딩과 KEP 서버간의 통신 상태 
    /// </summary>
    public class EventHealth
    {
        public string building_id;
        public string inspection_result_cd;
        public string inspection_result_val;
        public string inspection_datetime;
    }
}
