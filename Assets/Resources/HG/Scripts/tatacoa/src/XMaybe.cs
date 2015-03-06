using System;
using System.Collections;

namespace Tatacoa
{
	public abstract class XMaybe<A> : Maybe<A> {

	}

	public class XJust<A> : XMaybe<A> {

		A val;

		public XJust (A val)
		{
			this.val = val;
		}
		
		public override Maybe<B> FMap<B> (Func<A, B> f)
		{
			try {
				return Fn.XJust (f (val));
			}
			catch (Exception e) {
				return Fn.XNothing<B> (e);	
			}
		}

		public override Maybe<A> FMap (Action<A> f)
		{
			try {
				f (val);
				return this;
			}
			catch (Exception e) {
				return Fn.XNothing<A> (e);	
			}
		}

		public override Maybe<B> Bind<B> (Func<A, Maybe<B>> f)
		{
			try {
				return f (val);
			} 
			catch (Exception e) {
				return Fn.XNothing<B> (e);
			}
		}

		public override Either<B, A> ToEither<B> ()
		{
			return Fn.Right<B, A> (val);
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
	}

	public class XNothing<A> : XMaybe<A> {

		Exception e;

		public XNothing ()
		{
			this.e = new MaybeFailed ();
		}

		public XNothing (Exception e)
		{
			this.e = e != null ? e : new MaybeFailed () as Exception;
		}

		public override Maybe<B> FMap<B> (Func<A, B> f)
		{
			return new XNothing<B> (e);
		}

		public override Maybe<A> FMap (Action<A> f)
		{
			return this;
		}

		public override Maybe<A> XMap (Func<Exception, Exception> f)
		{
			e = f (e);
			return this;
		}

		public override Maybe<A> XMap (Action<Exception> f)
		{
			f (e);
			return this;
		}

		public override Maybe<B> Bind<B> (Func<A, Maybe<B>> f)
		{
			return new XNothing<B> (e);
		}

		public override Either<B, A> ToEither<B> ()
		{
			return new Neither<B, A> ();
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

		public override Exception exception {
			get {
				return e;
			}
		}

	}

	public static partial class Fn {

		public static Maybe<A> XMaybe<A> (A value, Exception e) {
			if (value == null) {
				return new XNothing<A> (e);
			}
			else {
				return new XJust<A> (value);
			}
		}

		public static Maybe<A> XMaybe<A> (A value) {
			if (value == null) {
				return new XNothing<A> (new NullReferenceException ());
			}
			else {
				return new XJust<A> (value);
			}
		}

		public static Func<A, Maybe<A>> XMaybe<A> () {
			return a => XMaybe (a);
		}

		public static XMaybe<A> XJust<A> (A value) {
			return new XJust<A> (value);
		}

		public static XMaybe<A> XNothing<A> (Exception exception) {
			return new XNothing<A> (exception);
		}

		public static XMaybe<A> XNothing<A> () {
			return new XNothing<A> ();
		}
	}
}