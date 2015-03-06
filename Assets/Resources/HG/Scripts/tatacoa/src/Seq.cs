using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Tatacoa
{
	public class Seq<A> : IEnumerable, Monad<A> {

		IEnumerable e = Fn.Enumerate (Fn.DoNothing);

		public Seq (IEnumerable<A> seq) {
			this.e = seq;
		}

		public Seq (A a) {
			this.e = Fn.Enumerate (a);
		}
		
		public Seq (IEnumerable seq) {
			this.e = seq;
		}

		public IEnumerable FMap (Func<A,IEnumerable> f) {
			var enu = GetEnumerator ();
			while (enu.MoveNext())
				yield return enu.Current;

			enu = f ((A)enu.Current).GetEnumerator ();
			while (enu.MoveNext())
				yield return enu.Current;
		}

		public IEnumerator GetEnumerator () {
			return e.GetEnumerator();
		}

		IEnumerable _FMap<B> (Func<A,B> f) {
			var enu = GetEnumerator ();
			while (enu.MoveNext())
				yield return enu.Current;
			
			yield return f ((A)enu.Current);
		}

		public virtual Seq<B> FMap<B> (Func<A,B> f) {
			return new Seq<B> (_FMap (f));
		}

		public Seq<A> FMap (Action<A> f) {
			return FMap (f.ToFunc ());
		}

		public Seq<A> FMap (Action f) {
			return FMap (f.ToFunc<A> ());
		}

		public Seq<A> XMap (Func<Exception,Exception> f) {
			return this;
		}

		public Seq<A> XMap (Action<Exception> f) {
			return XMap (f.ToFunc ());
		}

		public Seq<A> XMap (Action f) {
			return XMap (f.ToFunc<Exception> ());
		}

		IEnumerable _Bind<B> (Func<A,Seq<B>> f) {
			var enu = GetEnumerator ();
			while (enu.MoveNext())
				yield return enu.Current;

			enu = f ((A)enu.Current).GetEnumerator();
			while (enu.MoveNext())
				yield return enu.Current;
		}

		public Seq<B> Bind<B> (Func<A,Seq<B>> f) {
			return new Seq<B> (_Bind (f));
		}

		public Seq<B> Pure<B> (B value) {
			return new Seq<B> (value);
		}


		public Functor<A> Pure (A value)
		{
			return new Seq<A> (value);
		}

		Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
		{
			return FMap (f);
		}

		Functor<A> Functor<A>.XMap (Func<Exception, Exception> fx)
		{
			return this;
		}

		Monad<B> Monad<A>.Bind<B> (Func<A,Monad<B>> f) {
			return Bind (f as Func<A,Seq<B>>);
		}


	}

	public class XSeq<A> : Monad<A>, IEnumerable {

		Seq<Maybe<A>> s;
		Exception e;

		public XSeq (IEnumerable<A> seq) {
			s = new Seq<Maybe<A>> (seq.FMap (Fn.XMaybe<A> ()));
		}
		
		public XSeq (A a) {
			s = new Seq<Maybe<A>> (Fn.Enumerate (Fn.XMaybe (a)));
		}

		public XSeq (Maybe<A> m) {
			s = new Seq<Maybe<A>> (Fn.Enumerate (m));
		}
		
		public XSeq (Seq<A> seq) {
			s = seq.FMap (Fn.XMaybe<A> ());
		}

		public XSeq (IEnumerable seq) {
			s = new Seq<Maybe<A>> (seq);
		}
		
		public IEnumerator GetEnumerator () {
			return s.GetEnumerator();
		}
		
		public XSeq<B> FMap<B> (Func<A,B> f) {
			return new XSeq<B> (s.FMap (m => m.FMap (f)));
		}
		
		public XSeq<A> FMap (Action<A> f) {
			return FMap (f.ToFunc ());
		}
		
		public XSeq<A> FMap (Action f) {
			return FMap (f.ToFunc<A> ());
		}

		public XSeq<A> XMap (Func<Exception,Exception> f) {
			s = s.FMap (m => m.XMap (f));
			return this;
		}

		public XSeq<A> XMap (Action<Exception> f) {
			return XMap (f.ToFunc ());
		}

		public XSeq<A> XMap (Action f) {
			return XMap (f.ToFunc<Exception> ());
		}

		IEnumerable _Bind<B> (Func<A,XSeq<B>> f) {

			var enu = GetEnumerator ();
			while (enu.MoveNext())
				yield return enu.Current;
			
			var maybe = ((Maybe<A>)enu.Current);

			if (! maybe.IsNothing) {
				try {
					enu = f (maybe.value).GetEnumerator();
				}
				catch (Exception e) {
					enu = Fn.Enumerate(Fn.XNothing<B> (e)).GetEnumerator();
				}
				while (enu.MoveNext())
					yield return enu.Current;
			}
			else {
				yield return Fn.XNothing<B> (maybe.exception);
			}

		}

		public XSeq<B> Bind<B> (Func<A,XSeq<B>> f) {
			return new XSeq<B> (_Bind (f));
		}
		
		public XSeq<B> Pure<B> (B value) {
			return new XSeq<B> (value);
		}
		
		
		public Functor<A> Pure (A value)
		{
			return new XSeq<A> (value);
		}
		
		Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
		{
			return FMap (f);
		}
		
		Functor<A> Functor<A>.XMap (Func<Exception, Exception> fx)
		{
			return XMap (fx);
		}

		Monad<B> Monad<A>.Bind<B> (Func<A,Monad<B>> f) {
			return Bind (f as Func<A,XSeq<B>>);
		}



	}

	public static partial class Fn {

		public static Seq<A> Seq<A> (A value) {
			return new Seq<A> (value);
		}

		public static Seq<A> Seq<A> (Func<A> f) {
			return new Seq<A> (Fn.Enumerate(f));
		}

		public static Seq<A> Seq<A> (IEnumerable<A> e) {
			return new Seq<A> (e);
		}

		public static Seq<A> Seq<A> (Func<IEnumerable<A>> f) {
			return new Seq<A> (Fn.Enumerate (f));
		}

		public static Seq<A> Seq<A> (IEnumerable e) {
			return new Seq<A> (e);
		}

		public static Seq<Maybe<B>> FMap2<A,B> (this Seq<Maybe<A>> F, Func<A,B> f) {
			return F.FMap ((Maybe<A> m) => m.FMap (f));
		}

		public static Seq<Maybe<A>> FMap2<A> (this Seq<Maybe<A>> F, Action<A> f) {
			return F.FMap ((Maybe<A> m) => m.FMap (f));
		}

		public static Seq<Maybe<A>> FMap2<A> (this Seq<Maybe<A>> F, Action f) {
			return F.FMap ((Maybe<A> m) => m.FMap (f));
		}

		public static Seq<IEnumerable<B>> FMap2<A,B> (this Seq<IEnumerable<A>> F, Func<A,B> f) {
			return F.FMap ((IEnumerable<A> m) => (IEnumerable<B>)m.FMap (f).ToList());
		}

		public static Seq<IEnumerable<A>> FMap2<A> (this Seq<IEnumerable<A>> F, Action<A> f) {
			return F.FMap ((IEnumerable<A> m) => (IEnumerable<A>)m.FMap (f).ToList());
		}

		public static Seq<IEnumerable<A>> FMap2<A> (this Seq<IEnumerable<A>> F, Action f) {
			return F.FMap ((IEnumerable<A> m) => (IEnumerable<A>)m.FMap (f).ToList());
		}

		public static XSeq<A> XSeq<A> (A a) {
			return new XSeq<A> (a);
		}

		public static XSeq<A> XSeq<A> (Maybe<A> m) {
			return new XSeq<A> (m);
		}

		public static XSeq<A> XSeq<A> (Seq<A> s) {
			return new XSeq<A> (s);
		}

		public static XSeq<A> XSeq<A> (IEnumerable<A> e) {
			return new XSeq<A> (e);
		}

		public static XSeq<A> ToSeq<A> (Func<A> f) {
			return Fn.XSeq (Fn.Enumerate (f));
		}

	}
}