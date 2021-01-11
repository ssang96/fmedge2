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

        public override string ToString()
        {
            return $"type : {type}, building_id : {building_id}, inspection_result_cd : {inspection_result_cd }," +
                $"inspection_result_val : {inspection_result_val}, inspection_datetime : {inspection_datetime} ";
        }
    }
}
