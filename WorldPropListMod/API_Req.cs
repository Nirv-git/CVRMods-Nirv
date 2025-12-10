using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Networking.API.Responses;
using ABI_RC.Core.Networking.API.Responses.DetailsV2;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace WorldPropListMod
{
    internal static class ApiRequests
    {
        //Much of the credit goes to https://github.com/kafeijao/Kafe_CVR_Mods/blob/6e2b44b2ed3db22d21096ca53177be3a298a4f46/OSC/Utils/ApiRequests.cs#L7
        internal static async System.Threading.Tasks.Task<(string, string, string, string, bool, string, string, string)> RequestPropDetailsPageTask(string guid, bool firstAttempt)
        {//Name, URL, Author,tags, isPub, FileSize, UpdatedAt, Description
            if (Main.printAPIrequestsToConsole.Value) Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetching prop {guid} name...");
            BaseResponse<ContentSpawnableResponse> response;
            try
            {
                var payload = new { id = guid };
                response = await ApiConnection.MakeRequest<ContentSpawnableResponse>(ApiConnection.ApiOperation.PropDetail, payload, "2");
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"[API] Fetching prop {guid} name has Failed!");
                Main.Logger.Error(ex);
                return (null, null, null, null, false, null, null, null);
            }
            if (response == null)
            {
                Main.Logger.Msg(ConsoleColor.DarkYellow, $"[API] Fetching prop {guid} name has Failed! Response came back empty.");
                return (null, null, null, null, false, null, null, null);
            } 
            else if (response.Data.Name == null)
            {
                if (firstAttempt)
                {
                    if (Main.printAPIrequestsToConsole.Value) Main.Logger.Msg(ConsoleColor.DarkYellow, $"[API] Fetched prop {guid} returned null Data values, retrying!");
                    return await ApiRequests.RequestPropDetailsPageTask(guid, false);
                }
                else
                {
                    Main.Logger.Msg(ConsoleColor.DarkYellow, $"[API] Fetching prop {guid} name has Failed! Response returned null Data values twice!.");
                    return (null, null, null, null, false, null, null, null);
                }
            }
            if (Main.printAPIrequestsToConsole.Value) Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetched prop {guid} name successfully! Name: {response.Data.Name}");
            string tags = "";
            if (response.Data.Platforms.TryGetValue(Platforms.Pc_Standalone, out var platformData))
            {
                foreach (var tag in platformData.Tags)
                {
                    tags += tag + ", ";
                }
                //Main.Logger.Msg($"tags:{tags},\nisPub:{(response.Data.IsPublished ? "Published" : "Not Published")},\nFileSizeMB:{Utils.NumFormat(response.Data.FileSize/1048576f, 2)},\nUpdatedAt:{response.Data.UpdatedAt.ToString("yyyy'-'MM'-'dd")},\nDescription:{Utils.ReturnCleanASCII(response.Data.Description)},");
            }
            return (Utils.ReturnCleanASCII(response.Data.Name), response.Data.Image.ToString(), response.Data.Author.Name, tags, response.Data.Public,
                Utils.NumFormat(platformData.FileSize / 1048576f, 2), platformData.UpdatedAt.ToString("yyyy'-'MM'-'dd"), Utils.ReturnCleanASCII(response.Data.Description));
        }

        internal static async System.Threading.Tasks.Task<string> RequestPlayerDetailsPageTask(string guid)
        {
            if (Main.printAPIrequestsToConsole.Value) Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetching player {guid} name...");
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
            if (Main.printAPIrequestsToConsole.Value) Main.Logger.Msg(ConsoleColor.DarkCyan, $"[API] Fetched player {guid} name successfully! Name: {response.Data.Name}");
            return response.Data.Name;
        }
    }
}
