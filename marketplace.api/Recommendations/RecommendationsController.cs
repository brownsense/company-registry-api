using System;
using System.Linq;
using marketplace.api.Data;
using marketplace.api.Recommendations;
using Marketplace.Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace  MarketPlace.Api.Entities
{

    public class RecommendationsController : Controller
    {
        private MarketplaceContext db;
        private AuthorizationService authorizationService;
        private readonly QueryHelper queryHelper;

        public RecommendationsController(
            MarketplaceContext object1,
            AuthorizationService object2,
            QueryHelper queryHelper)
        {
            this.db = object1;
            this.authorizationService = object2;
            this.queryHelper = queryHelper;
        }

        public ActionResult Rate(int entityId, Rating rating)
        {
            var userId = authorizationService.GetUserId();

            var foundRating = db.Ratings.SingleOrDefault(r => r.Entity == entityId && r.RatedBy == userId);
            if(foundRating == null)
            {
                rating.RatedBy = userId;
                rating.Entity = entityId;
                db.Ratings.Add(rating);
                db.SaveChanges();
                return Created("", rating);
            }
            else
            {
                foundRating.Comment = rating.Comment;
                foundRating.RatingValue = rating.RatingValue;
                db.SaveChanges();
                return Ok();
            }
        }

        public ActionResult QueryUserRatings(int userId)
        {
            var theReturn = queryHelper.Query(db.Ratings.Where(r => r.RatedBy == userId), 10, 0, r => true);
            return Json(theReturn);
        }

        public ActionResult QueryEntityRatings(int entityId)
        {
            var theReturn = queryHelper.Query(db.Ratings.Where(r => r.Entity == entityId), 10, 0, r => true);
            return Json(theReturn);
        }

        public ActionResult GetEntityRatingsSummary(int entityId)
        {
            var ratingsValues = new[] { Stars.One, Stars.Two, Stars.Three, Stars.Four, Stars.Five };
            var ratingsSummaryEntries = ratingsValues.GroupJoin(
                db.Ratings.Where(r => r.Entity == entityId),
                r => r,
                r => r.RatingValue,
                (stars, ratings) => new { Stars = stars, Count = ratings.Count() }
            ).ToArray();
            var ratingSummary = new RatingSummary {
                OneStar = ratingsSummaryEntries.Single(s => s.Stars == Stars.One).Count,
                TwoStars = ratingsSummaryEntries.Single(s => s.Stars == Stars.Two).Count,
                ThreeStars = ratingsSummaryEntries.Single(s => s.Stars == Stars.Three).Count,
                FourStars = ratingsSummaryEntries.Single(s => s.Stars == Stars.Four).Count,
                FiveStars = ratingsSummaryEntries.Single(s => s.Stars == Stars.Five).Count
            };
            return Json(ratingSummary);
        }
    }
}
