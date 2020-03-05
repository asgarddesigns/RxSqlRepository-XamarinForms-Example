using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using RxSqlRepository.Model;
using SQLite;

namespace RxSqlRepository.Data
{
    public class BeerInMemoryDataSource : IDataSource
    {
        private readonly ISubject<IReadOnlyList<BeerType>> _itemSubject =
            new BehaviorSubject<IReadOnlyList<BeerType>>(new List<BeerType>());

        private readonly SQLiteConnection _sqLiteConnection;

        public BeerInMemoryDataSource()
        {
            var options = new SQLiteConnectionString("rx-repository-demo.db");
            var conn = new SQLiteConnection(options);
            conn.EnableWriteAheadLogging();
            _sqLiteConnection = conn;

            _sqLiteConnection.CreateTable<BeerType>();
            _sqLiteConnection.Insert(new BeerType {Name = "Brown Ale"});
        }

        public IObservable<IReadOnlyList<BeerType>> FetchAll()
        {
            FetchAllFromDb();
            return _itemSubject;
        }

        private void FetchAllFromDb()
        {
            var result = _sqLiteConnection.Table<BeerType>().ToList();
            _itemSubject.OnNext(result);
        }

        public IObservable<BeerType?> FetchById(long id)
        {
            FetchAllFromDb();
            return _itemSubject.Select(list => list.FirstOrDefault(type => type.Id == id));
        }

        public void Add(BeerType beerType)
        {
            _sqLiteConnection.Insert(beerType);
            FetchAllFromDb();
        }
    }
}