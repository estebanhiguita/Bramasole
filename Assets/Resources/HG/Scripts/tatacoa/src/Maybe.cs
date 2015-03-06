using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tatacoa
{
	public abstract class Maybe <A> : Monad<A> {

		public abstract bool IsNothing {get;}
		public abstract A value {get;}
		public abstract Maybe<B> FMap<B> (Func<A,B> f);
		public abstract Maybe<A> FMap (Action<A> f);
		public virtual Maybe<A> XMap (Func<Exception,Exception> f) {return this;}
		public virtual Maybe<A> XMap (Action<Exception> f) {return this;}
		public virtual Maybe<A> XMap (Action f) {return XMap (f.ToAction<Exception> ());}
		public abstract Maybe<B> Bind<B> (Func<A,Maybe<B>> f);

		public abstract Either<B,A> ToEither<B> ();

		public Maybe<A> FMap (Action f) {
			return FMap (f.ToAction<A> ());
		}

		public Maybe<A> Pure (A a) {
			return Fn.Maybe (a);
		}

		Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
		{
			return FMap (f);
		}

		Functor<A> Functor<A>.XMap (Func<Exception, Exception> fx)
		{
			return XMap (fx);
		}

		Functor<A> Applicative<A>.Pure (A a)
		{
			return Pure (a);
		}

		public static bool operator true (Maybe<A> m) {
			return ! m.IsNothing;
		}

		public static bool operator false (Maybe<A> m) {
			return m.IsNothing;
		}

		public static bool operator ! (Maybe<A> m) {
			return m.IsNothing;
		}

		Monad<B> Monad<A>.Bind<B> (Func<A, Monad<B>> f)
		{
			if (IsNothing)
				return Fn.Nothing<B>();
			else
				return f (value);
		}

		public virtual Exception exception {
			get {
				return default (Exception);
			}
		}
	}

	public class Just<A> : Maybe<A> {
		
		public readonly A val;
		
		public Just (A val) {
			this.val = val;
		}
		
		public override Maybe<B> FMap <B> (Func <A, B> f)
		{
			return Fn.Maybe (f (val));
		}

		public override Maybe<A> FMap (Action<A> f)
		{
			f (val);
			return this;
		}

		public override Maybe<A> XMap (Func<Exception, Exception> f)
		{
			return this;
		}

		public override Maybe<A> XMap (Action<Exception> f)
		{
			throw new NotImplementedException ();
		}
		
		public override bool IsNothing {
			get {
				return false;
			}
		}
		
		public override A value {
			get {
				return val;
			}
		}

		public override Maybe<B> Bind<B> (Func<A, Maybe<B>> f)
		{
			return f (val);
		}

		public override Either<B, A> ToEither<B> ()
		{
			return Fn.MaybeRight<B, A> (val);
		}

	}

	public class Nothing<A> : Maybe<A> {

		public override Maybe<B> FMap<B> (Func<A, B> f)
		{
			//Debug.Log ("Nothing Happened");
			return new Nothing<B> ();
		}

		public override Maybe<A> FMap (Action<A> f)
		{
			return this;
		}

		public override bool IsNothing {
			get {
				return true;
			}
		}

		public override A value {
			get {
				return default (A);
			}
		}

		public override Maybe<B> Bind<B> (Func<A, Maybe<B>> f)
		{
			return this;
		}

		public override Either<B, A> ToEither<B> ()
		{
			return new Neither<B, A> ();
		}
	}

	public class TMaybe {
		private static TMaybe _i;
		public static TMaybe i {
			get {
				if (_i != null) _i = new TMaybe();
				return _i;
			}
		}
		private TMaybe (){}
	}

	public class MaybeFailed : Exception
	{
		public MaybeFailed()
		{
		}
		
		public MaybeFailed(string message)
			: base(message)
		{
		}
		
		public MaybeFailed(string message, Exception inner)
			: base(message, inner)
		{
		}
	}


	public static partial class Fn {
		
		public static Maybe<A> Just<A> (A a) {
			return new Just<A> (a);
		}
		
		public static Maybe<A> Nothing<A> () {
			return new Nothing<A> ();
		}

		public static Maybe<A> Maybe<A> (A a) {
			if (a == null) {
				//Debug.Log ("Maybe Failed");
				return new Nothing<A> ();
			}
			else
				return new Just<A> (a);
		}

		public static Maybe<string> Maybe (string s) {
			return s == null || s == "" ? new Nothing<string> () as Maybe<string> : new Just<string> (s) as Maybe<string>;
		}

		public static Maybe<string> Maybe (string s, Func<string> defaultValue) {
			return s == null || s == "" ? new Just<string> (defaultValue ()) as Maybe<string> : new Just<string> (s) as Maybe<string>;
		}

		public static Maybe<A> Maybe<A> (A a, Func<A> defaultValue) {
			return Equals (a, default(A)) ? new Just<A> (defaultValue ()) as Maybe<A> : new Just<A> (a) as Maybe<A>;
		}

		public static Func<A,Maybe<A>> Maybe<A> () {
			return a => Maybe (a);
		}


		// FUNCTOR
		
		//FMap :: (a -> b) -> Maybe a -> Maybe b
		public static Maybe<B> FMap<A,B> (Func<A,B> f, Maybe<A> F) {
			return F.FMap (f);
		}

		//FMap :: (a -> void) -> Maybe a -> Maybe a
		public static Maybe<A> FMap<A> (Action<A> f, Maybe<A> F) {
			return F.FMap (f);
		}
		
		//FMap :: (a -> b) -> (Maybe a -> Maybe b)
		public static Func<Maybe<A>,Maybe<B>> FMap<A,B> (TMaybe _, Func<A,B> f) {
			return F => F.FMap (f);
		}

		//FMap :: (a -> b) -> (Maybe a -> Maybe b)
		public static Func<Maybe<A>,Maybe<A>> FMap<A> (TMaybe _, Action<A> f) {
			return F => F.FMap (f);
		}

		// APPLICATIVE

		//Pure :: a -> Maybe a
		public static Maybe<A> Pure<A> (TMaybe _, A a) {
			return Maybe (a);
		}

		//Apply :: Maybe (a -> b) -> Maybe a -> Maybe b
		public static Maybe<B> Apply<A,B> (this Maybe<Func<A,B>> mf, Maybe<A> m) {
			return mf.IsNothing ? new Nothing<B>() : m.FMap (mf.value);
		}

		//Apply :: Maybe (a -> b) -> Maybe a -> Maybe b
		public static Maybe<A> Apply<A> (this Maybe<Action<A>> mf, Maybe<A> m) {
			return mf.IsNothing ? new Nothing<A>() : m.FMap (mf.value);
		}

		//Apply :: Maybe (a -> b) -> Maybe a -> Maybe b
		public static Maybe<B> up<A,B> (this Maybe<Func<A,B>> mf, Maybe<A> m) {
			return mf ? new Nothing<B>() : m.FMap (mf.value);
		}
		
		//Apply :: Maybe (a -> b) -> Maybe a -> Maybe b
		public static Maybe<A> up<A> (this Maybe<Action<A>> mf, Maybe<A> m) {
			return mf ?  m.FMap (mf.value) : new Nothing<A>();
		}

		public static Maybe<B> Apply<A,B> (this Func<A,B> f, Maybe<A> m) {
			return m.FMap (f);
		}

		public static Maybe<A> Apply<A> (this Action<A> f, Maybe<A> m) {
			return m.FMap (f);
		}

		public static Maybe<B> up<A,B> (this Func<A,B> f, Maybe<A> m) {
			return m.FMap (f);
		}
		
		public static Maybe<A> up<A> (this Action<A> f, Maybe<A> m) {
			return m.FMap (f);
		}

		//Apply :: Maybe (a -> b) -> (Maybe a -> Maybe b)
		public static Func<Maybe<Func<A,B>>,Maybe<B>> Apply<A,B> (TMaybe _, Maybe<A> m) {
			return mf => mf ? new Nothing<B>() : m.FMap (mf.value);
		}

		//Apply :: Maybe (a -> void) -> (Maybe a -> Maybe a)
		public static Func<Maybe<Action<A>>,Maybe<A>> Apply<A> (TMaybe _, Maybe<A> m) {
			return mf => mf ? new Nothing<A>() : m.FMap (mf.value);
		}

		//MONAD

		// Bind :: (a -> Maybe b) -> Maybe a -> Maybe b
		public static Maybe<B> Bind<A,B> (Func<A,Maybe<B>> f, Maybe<A> m) {
			return m.Bind (f);
		}

		// Bind :: (a -> Maybe b) -> Maybe a -> Maybe b
		public static Func<Maybe<A>,Maybe<B>> Bind<A,B> (Func<A,Maybe<B>> f) {
			return m => Bind(f, m);
		}

		// Bind :: (a -> Maybe b) -> Maybe a -> Maybe b
		public static Func<Func<A,Maybe<B>>,Func<Maybe<A>,Maybe<B>>> Bind<A,B> () {
			return f => m => Bind(f, m);
		}

		//Utility
		public static Maybe<A> Flatten<A> (this Maybe<Maybe<A>> m) {
			return m.IsNothing ? new Nothing<A>() : m.value;
		}

		public static Seq<Maybe<A>> Flatten<A> (this Seq<Maybe<Maybe<A>>> s) {
			return s.FMap ((Maybe<Maybe<A>> m) =>  Flatten (m));
		}

		public static Maybe<IEnumerable<B>> FMap2<A,B> (this Maybe<IEnumerable<A>> m, Func<A,B> f)
		{
			return m.FMap ((IEnumerable<A> e) => e.FMap(f));
		}

		public static Maybe<List<B>> Map2<A,B> (this Maybe<IEnumerable<A>> m, Func<A,B> f)
		{
			return m.FMap ((IEnumerable<A> e) => e.Map(f));
		}

		public static Maybe<List<A>> Map2<A> (this Maybe<IEnumerable<A>> m, Action<A> f)
		{
			return m.Map2 (f.ToFunc());
		}

	}
}