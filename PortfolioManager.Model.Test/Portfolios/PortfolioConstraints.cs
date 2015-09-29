using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using NUnit.Framework.Constraints;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{

    public static class PortfolioConstraint
    {
        public static ShareParcelEqual Equals(ShareParcel expected)
        {
            return new ShareParcelEqual(expected);
        }

        public static ShareParcelCollectionEqual Equals(ICollection<ShareParcel> expected)
        {
            return new ShareParcelCollectionEqual(expected);
        }

        public static IncomeReceivedEqualConstraint Equals(IncomeReceived expected)
        {
            return new IncomeReceivedEqualConstraint(expected);
        }

        public static IncomeReceivedCollectionEqualConstraint Equals(ICollection<IncomeReceived> expected)
        {
            return new IncomeReceivedCollectionEqualConstraint(expected);
        }
    }

    public interface IEntityWriter<T>
                where T: IEntity
    {
        void Write(MessageWriter writer, T entity);
    }

    public class GenericEntityEqualConstraint<T, C, W> : Constraint 
        where T: IEntity
        where C: IEqualityComparer<T>, new()
        where W: IEntityWriter<T>, new()
    {
        private readonly T _Expected;
        private readonly C _EntityComparer;
        private readonly W _EntityWriter;

        public GenericEntityEqualConstraint(T expected)
        {
            _Expected = expected;
            _EntityComparer = new C();
            _EntityWriter = new W();
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;

            if (actual is T)
                return _EntityComparer.Equals(_Expected, (T)actual);
            else
                return false; 
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
           if (actual is T)
                _EntityWriter.Write(writer, (T)actual);
            else
                writer.WriteActualValue(actual); 
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            _EntityWriter.Write(writer, (T)_Expected);
        }

    }

    public class GenericEntityCollectionEqualConstraint<T, C, W> : Constraint 
        where T: IEntity
        where C: IEqualityComparer<T>, new()
        where W: IEntityWriter<T>, new()
    {
        private readonly ICollection<T> _Expected;
        private readonly C _EntityComparer;
        private readonly W _EntityWriter;

        public GenericEntityCollectionEqualConstraint(ICollection<T> expected)
            : base(expected)
        {
            _Expected = expected;
            _EntityComparer = new C();
            _EntityWriter = new W();
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            bool found;

            if (actual is IEnumerable<T>)
              {
                  List<T> expectedEntities = _Expected.ToList();
                  IEnumerable<T> actualEntities = actual as IEnumerable<T>;

                  foreach (T actualEntity in actualEntities)
                  {
                      found = false;
                      foreach (T expectedEntity in expectedEntities)
                      {
                          if (_EntityComparer.Equals(expectedEntity, actualEntity))
                          {
                              expectedEntities.Remove(expectedEntity);
                              found = true;
                              break;
                          }
                      }

                      if (!found)
                          return false;
                  }

                  if (expectedEntities.Count > 0)
                      return false;

                  return true;
              } 

            return false;
        } 


        public override void WriteDescriptionTo(MessageWriter writer)
        {
            WriteValue(writer, _Expected);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (actual is IEnumerable<T>)
                WriteValue(writer, actual as IEnumerable<T>);
            else
                writer.WriteActualValue(actual);
        }

        private void WriteValue(MessageWriter writer, IEnumerable<T> entities)
        {
            int count = 0;

                writer.Write("<");
                foreach (T entity in entities)
                {
                    if (count > 0)
                        writer.Write(",\n              ");

                    _EntityWriter.Write(writer, entity);

                    count++;
                } 
            writer.Write(">");
        } 
    } 

}
