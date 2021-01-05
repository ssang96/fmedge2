namespace fmedge.Model
{
    /// <summary>
    /// KEP 서버에서 전달하는 이벤트 데이터
    /// </summary>
    public class EventStatus
    {
        public string building_id;
        public string tag_id;
        public string present_val;
        public string point_address;
        public string protocol_type;
        public string create_datetime;
    }
}
