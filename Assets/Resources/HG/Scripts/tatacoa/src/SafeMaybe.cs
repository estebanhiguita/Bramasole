using System;
using System.Collections;

namespace Tatacoa
{
	public abstract class SafeMaybe<A> : Either<Exception,A> {

	}

	public class JustRight<A> : SafeMaybe<A> {

		A val;

		public JustRight (A value) {
			val = value;
		}

		public override Either<Exception, C> Bind<C> (Func<A, Either<Exception, C>> f)
		{
			try {
				return f (val);
			}
			catch (Exception e) {
				return new LeftException<C> (e);
			}
		}

		public override Either<C, A> BindL<C> (Func<Exception, Either<C, A>> f)
		{
			return new Right<C, A> (val);
		}

		public override Either<Exception, C> FMap<C> (Func<A, C> f)
		{
			try {
				return new JustRight<C> (f (val));
			}
			catch (Exception e) {
				return new Left<Exception, C> (e);
			}
		}

		public override Either<C, A> MapL<C> (Func<Exception, C> f)
		{
			return new Right<C, A> (val);
		}

		public override Either<A, Exception> Swap ()
		{
			return new Left<A, Exception> (val);
		}

		public override Either<Exception, A> IfFailed (Action f)
		{
			return this;
		}

		public override Either<Exception, A> IfFailed (Action<Exception> f)
		{
			return this;
		}

		public override Exception left {
			get {
				return default (Exception);
			}
		}

		public override A right {
			get {
				return val;
			}
		}

		public override bool isRight {
			get {
				return true;
			}
		}

		public override bool isLeft {
			get {
				return false;
			}
		}
	}

	public class LeftException<A> : SafeMaybe<A> {

		Exception e;

		public LeftException (Exception exception) {
			this.e = exception;
		}

		public override Either<Exception, C> Bind<C> (Func<A, Either<Exception, C>> f)
		{
			return new Left<Exception, C> (e);
		}

		public override Either<C, A> BindL<C> (Func<Exception, Either<C, A>> f)
		{
			return f (e);
		}

		public override Either<Exception, C> FMap<C> (Func<A, C> f)
		{
			return new Left<Exception, C> (e);
		}

		public override Either<C, A> MapL<C> (Func<Exception, C> f)
		{
			return new Left<C, A> (f (e));
		}

		public override Either<A, Exception> Swap ()
		{
			return new Right<A, Exception> (e);
		}

		public override Either<Exception, A> IfFailed (Action f)
		{
			f ();
			return this;
		}

		public override Either<Exception, A> IfFailed (Action<Exception> f)
		{
			f (e);
			return this;
		}

		public override Exception left {
			get {
				return e;
			}
		}

		public override A right {
			get {
				return default (A);
			}
		}

		public override bool isRight {
			get {
				return false;
			}
		}

		public override bool isLeft {
			get {
				return true;
			}
		}
	}

	public static partial class Fn {

		public static SafeMaybe<A> ExceptionEither<A> (A value) {
			if (value == null) {
				return new LeftException<A> (new NullReferenceException ());
			}
			else {
				return new JustRight<A> (value);
			}

		}

		public static SafeMaybe<A>JustRight<A> (A value) {
			return new JustRight<A> (value);
		}

		public static SafeMaybe<A> LeftException<A> (Exception exception) {
			return new LeftException<A> (exception);
		}
	}
}