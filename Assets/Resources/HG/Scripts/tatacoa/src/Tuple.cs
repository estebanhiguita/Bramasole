using UnityEngine;
using System;
using System.Collections;

namespace Tatacoa
{
	public class Tuple<A,B> : Functor<B> {

		public A fst;
		public B snd;

		public Tuple (A a, B b) {
			fst = a;
			snd = b;
		}

		public Tuple() {

		}

		Functor<C> Functor<B>.FMap<C> (Func<B, C> f)
		{
			return new Tuple<A,C> (fst, f (snd));
		}

		public Functor<B> XMap (Func<Exception, Exception> fx)
		{
			throw new NotImplementedException ();
		}
	}

	public class Tuple<A,B,C> : Functor<C> {
		
		public A fst;
		public B snd;
		public C third;
		
		public Tuple (A a, B b, C c) {
			fst = a;
			snd = b;
			third = c;
		}

		public Tuple() {
			
		}
		
		Functor<D> Functor<C>.FMap<D> (Func<C,D> f)
		{
			return new Tuple<A,B,D> (fst, snd, f (third));
		}

		public Functor<C> XMap (Func<Exception, Exception> fx)
		{
			throw new NotImplementedException ();
		}
	}

	public class Tuple<A,B,C,D> : Functor<D> {
		
		public A fst;
		public B snd;
		public C third;
		public D forth;
		
		public Tuple (A a, B b, C c, D d) {
			fst = a;
			snd = b;
			third = c;
			forth = d;
		}

		public Tuple() {
			
		}
		
		Functor<E> Functor<D>.FMap<E> (Func<D,E> f)
		{
			return new Tuple<A,B,C,E> (fst, snd, third, f (forth));
		}

		public Functor<D> XMap (Func<Exception, Exception> fx)
		{
			throw new NotImplementedException ();
		}
	}


	public static partial class Fn {
		public static Tuple<A,B> Tuple<A,B> (A a, B b) {
			return new Tuple<A,B> (a, b);
		}

		public static Tuple<A,B,C> Tuple<A,B,C> (A a, B b, C c) {
			return new Tuple<A,B,C> (a, b, c);
		}

		public static Tuple<A,B,C,D> Tuple<A,B,C,D> (A a, B b, C c, D d) {
			return new Tuple<A,B,C,D> (a, b, c, d);
		}

		//Curried
		public static Func<A,Func<B,Tuple<A,B>>> Tuple<A,B> () {
			return a => b => Fn.Tuple (a, b);
		}
	}
}