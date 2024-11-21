using PluginSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCONSOLE.DataBase.Tables
{
    [TableName("seats")]
    public class Seat : TableBase
    {
        [PrimaryKey]
        public int id { get; set; }
        public int map_id { get; set; }
        public int row { get; set; }
        public int col { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public bool visible { get; set; }

        [FieldOmite]
        public int status { get; set; }

        public static List<Seat> Get(int map_id)
        {
            return MYSQL.Query<Seat>($"SELECT * FROM seats WHERE map_id='{map_id}' ORDER BY id");
        }

        public static List<Seat> Get(int map_id, int func_room_id, DateOnly date)
        {
            return MYSQL.Query<Seat>($@"
                    SELECT 
                       s.id, 
                       s.map_id, 
                       s.row,
                       s.col,
                       s.type, 
                       s.name, 
                       s.visible,
                      COALESCE(fs.status, 
                        CASE 
                            WHEN s.type = {(int)SeatType.TRADICIONAL} THEN {(int)SeatStatus.TRADICIONAL}
                            WHEN s.type = {(int)SeatType.DISCAPACITADO} THEN {(int)SeatStatus.DISCAPACITADO}
                            WHEN s.type = {(int)SeatType.DESABILITADO} THEN {(int)SeatStatus.DESABILITADO}
                            ELSE {(int)SeatStatus.TRADICIONAL}
                        END
                    ) AS status
                    FROM seats AS s
                    LEFT JOIN function_seats AS fs ON s.id = fs.seat_id AND fs.func_room_id = '{func_room_id}' AND fs.date='{date.ToString("yyyy-MM-dd")}'
                    WHERE s.map_id = '{map_id}'
                    ORDER BY s.id ASC");
        }
    }
}
