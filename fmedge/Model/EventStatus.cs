namespace fmedge.Model
{
    /// <summary>
    /// KEP 서버에서 전달하는 이벤트 데이터
    /// </summary>
    public class EventStatus
    {
        public string type { get; set; }
        public string building_id { get; set; }
        public string tag_id { get; set; }
        public string present_val { get; set; }
        public string point_address { get; set; }
        public string protocol_type { get; set; } 
        public string create_datetime { get; set; }

        public override string ToString()
        {
            return $"type : {type}, building_id : {building_id}, tag_id : {tag_id }," +
                $"present_val : {present_val}, point_address : {point_address}, " +
                $"protocol_type : {protocol_type}, create_datetime : {create_datetime} ";
        }
    }
}
