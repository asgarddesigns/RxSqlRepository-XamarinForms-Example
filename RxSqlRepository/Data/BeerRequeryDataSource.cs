using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using RxSqlRepository.Model;
using SQLite;

namespace RxSqlRepository.Data
{
    public class BeerRequeryDataSource : IDataSource
    {
        private ISubject<ISet<long>> _itemSubject = new Subject<ISet<long>>();

        private readonly SQLiteConnection _sqLiteConnection;

        public BeerRequeryDataSource()
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
            return DataChangedStream(new HashSet<long>())
                .Select(_ => _sqLiteConnection.Table<BeerType>().ToList());
        }

        private IObservable<ISet<long>> DataChangedStream(ISet<long> initial)
        {
            return _itemSubject.StartWith(initial);
        }

        public IObservable<BeerType?> FetchById(long id)
        {
            return DataChangedStream(new HashSet<long> {id})
                .Select(_ => _sqLiteConnection.Table<BeerType>().FirstOrDefault(type => type.Id == id));
        }

        public void Add(BeerType beerType)
        {
            _sqLiteConnection.Insert(beerType);
            ItemChanged(beerType.Id);
        }

        private void ItemChanged(long item)
        {
            _itemSubject.OnNext(new HashSet<long> {item});
        }
    }
}