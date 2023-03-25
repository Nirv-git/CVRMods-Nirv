using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Networking.API.Responses;


namespace WorldPropListMod
{
    internal static class ApiRequests
    {
        //Much of the credit goes to https://github.com/kafeijao/Kafe_CVR_Mods/blob/6e2b44b2ed3db22d21096ca53177be3a298a4f46/OSC/Utils/ApiRequests.cs#L7
        internal static async System.Threading.Tasks.Task<(string, string)> RequestPropDetailsPageTask(string guid)
        {
            Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetching prop {guid} name...");
            BaseResponse<SpawnableDetailsResponse> response;
            try
            {
                var payload = new { id = guid };
                response = await ApiConnection.MakeRequest<SpawnableDetailsResponse>(ApiConnection.ApiOperation.PropDetail, payload);
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"[API] Fetching prop {guid} name has Failed!");
                Main.Logger.Error(ex);
                return (null,null);
            }
            if (response == null)
            {
                Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetching prop {guid} name has Failed! Response came back empty.");
                return (null, null);
            }
            Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetched prop {guid} name successfully! Name: {response.Data.Name}");
            return (response.Data.Name, response.Data.ImageUrl);
        }

        internal static async System.Threading.Tasks.Task<string> RequestPlayerDetailsPageTask(string guid)
        {
            Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetching player {guid} name...");
            BaseResponse<UserDetailsResponse> response;
            try
            {
                var payload = new { userID = guid };
                response = await ApiConnection.MakeRequest<UserDetailsResponse>(ApiConnection.ApiOperation.UserDetails, payload);
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"[API] Fetching player {guid} name has Failed!");
                Main.Logger.Error(ex);
                return null;
            }
            if (response == null)
            {
                Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetching player {guid} name has Failed! Response came back empty.");
                return null;
            }
            Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetched player {guid} name successfully! Name: {response.Data.Name}");
            return response.Data.Name;
        }
    }
}
