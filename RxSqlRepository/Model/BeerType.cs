using SQLite;

namespace RxSqlRepository.Model
{
    [Table(nameof(BeerType))]
    public class BeerType
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}