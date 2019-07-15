using System.Collections.Generic;
using ConnectApp.Api;
using ConnectApp.Models.Api;
using ConnectApp.Models.Model;
using ConnectApp.Models.State;
using Unity.UIWidgets.Redux;
using UnityEngine;

namespace ConnectApp.redux.actions {
    public class PopularSearchArticleSuccessAction : RequestAction {
        public List<PopularSearch> popularSearchArticles;
    }
    
    public class PopularSearchUserSuccessAction : RequestAction {
        public List<PopularSearch> popularSearchUsers;
    }

    public class StartSearchArticleAction : RequestAction {
        public string keyword;
    }

    public class SearchArticleSuccessAction : BaseAction {
        public string keyword;
        public int pageNumber;
        public FetchSearchArticleResponse searchArticleResponse;
    }

    public class SearchArticleFailureAction : BaseAction {
        public string keyword;
    }

    public class ClearSearchResultAction : BaseAction {
    }

    public class ClearSearchFollowingResultAction : BaseAction {
    }

    public class SaveSearchArticleHistoryAction : BaseAction {
        public string keyword;
    }

    public class DeleteSearchArticleHistoryAction : BaseAction {
        public string keyword;
    }

    public class DeleteAllSearchArticleHistoryAction : BaseAction {
    }
    
    public class StartSearchUserAction : RequestAction {
    }

    public class SearchUserSuccessAction : BaseAction {
        public string keyword;
        public int pageNumber;
        public List<User> users;
        public bool hasMore;
    }

    public class SearchUserFailureAction : BaseAction {
        public string keyword;
    }
    
    public class StartSearchFollowingAction : RequestAction {
    }

    public class SearchFollowingSuccessAction : BaseAction {
        public string keyword;
        public int pageNumber;
        public List<User> users;
        public bool hasMore;
    }

    public class SearchFollowingFailureAction : BaseAction {
        public string keyword;
    }

    public static partial class Actions {
        public static object searchArticles(string keyword, int pageNumber) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return SearchApi.SearchArticle(keyword, pageNumber)
                    .Then(searchArticleResponse => {
                        dispatcher.dispatch(new UserMapAction {userMap = searchArticleResponse.userMap});
                        dispatcher.dispatch(new TeamMapAction {teamMap = searchArticleResponse.teamMap});
                        dispatcher.dispatch(new SearchArticleSuccessAction {
                            keyword = keyword,
                            pageNumber = pageNumber,
                            searchArticleResponse = searchArticleResponse
                        });
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new SearchArticleFailureAction {keyword = keyword});
                        Debug.Log(error);
                    });
            });
        }

        public static object popularSearchArticle() {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return SearchApi.PopularSearchArticle()
                    .Then(popularSearchArticles => {
                        dispatcher.dispatch(new PopularSearchArticleSuccessAction {
                            popularSearchArticles = popularSearchArticles
                        });
                    })
                    .Catch(error => Debug.Log($"{error}"));
            });
        }

        public static object searchUsers(string keyword, int pageNumber) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return SearchApi.SearchUser(keyword, pageNumber)
                    .Then(searchUserResponse => {
                        dispatcher.dispatch(new FollowMapAction {
                            followMap = searchUserResponse.followingMap
                        });
                        var userMap = new Dictionary<string, User>();
                        (searchUserResponse.users ?? new List<User>()).ForEach(searchUser => {
                            if (userMap.ContainsKey(key: searchUser.id)) {
                                userMap[key: searchUser.id] = searchUser;
                            }
                            else {
                                userMap.Add(key: searchUser.id, value: searchUser);
                            }
                        });
                        dispatcher.dispatch(new UserMapAction {userMap = userMap});
                        dispatcher.dispatch(new SearchUserSuccessAction {
                            keyword = keyword,
                            pageNumber = pageNumber,
                            users = searchUserResponse.users,
                            hasMore = searchUserResponse.hasMore
                        });
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new SearchUserFailureAction {keyword = keyword});
                        Debug.Log(error);
                    });
            });
        }

        public static object searchFollowings(string keyword, int pageNumber) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return SearchApi.SearchUser(keyword, pageNumber)
                    .Then(searchFollowingResponse => {
                        dispatcher.dispatch(new SearchFollowingSuccessAction {
                            keyword = keyword,
                            pageNumber = pageNumber,
                            users = searchFollowingResponse.users,
                            hasMore = searchFollowingResponse.hasMore
                        });
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new SearchFollowingFailureAction {keyword = keyword});
                        Debug.Log(error);
                    });
            });
        }
        
        public static object popularSearchUser() {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return SearchApi.PopularSearchUser()
                    .Then(popularSearchUsers => {
                        dispatcher.dispatch(new PopularSearchUserSuccessAction {
                            popularSearchUsers = popularSearchUsers
                        });
                    })
                    .Catch(error => Debug.Log($"{error}"));
            });
        }
    }
}