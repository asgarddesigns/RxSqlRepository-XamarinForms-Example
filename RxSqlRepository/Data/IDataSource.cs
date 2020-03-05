using System;
using System.Collections.Generic;
using RxSqlRepository.Model;

namespace RxSqlRepository
{
    internal interface IDataSource
    {
        IObservable<IReadOnlyList<BeerType>> FetchAll();

        IObservable<BeerType?> FetchById(long id);
        
        void Add(BeerType beerType);
    }
}