using UnityEngine;
using System;
using System.Collections;

namespace Tatacoa
{
	public class Lazy<A> : Applicative<A> {

		Func<A> a;

		public A value {
			get {
				return a();
			}
		}

		public Lazy<B> FMap<B> (Func<A,B> f) {
			return new Lazy<B> (() => f ( a() ));
		}

		public Lazy<A> FMap (Action<A> f) {
			return FMap (f.ToFunc ());
		}

		public Lazy(A val){
			a = () => val;
		}

		public Lazy(Func<A> f){
			a = f;
		}

		Functor<A> Applicative<A>.Pure (A a)
		{
			return new Lazy<A> (a);
		}

		Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
		{
			return FMap<B> (f);
		}

		public Functor<A> XMap (Func<Exception, Exception> fx)
		{
			throw new NotImplementedException ();
		}
	}

	public static partial class Fn {

		public static Lazy<B> FMap<A,B> (Func<A,B> f, Lazy<A> F){
			return F.FMap (f);
		}
	}
}