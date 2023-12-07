using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using Moeller.TheCli.Domain.Personio.Common;
using Moeller.TheCli.Domain.Personio.Configuration;
using Moeller.TheCli.Domain.Personio.Models;
using Moeller.TheCli.Domain.Personio.Models.Attributes;
using Moeller.TheCli.Domain.Personio.Models.Request;
using Moeller.TheCli.Domain.Personio.Models.Response;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio
{
    public class PersonioClient
    {
        #region Properties

        private readonly PersonioClientOptions _options;

        private string _token { get; set; }
        private DateTime? _tokenValidUntilUtc { get; set; }
        private bool _isValidToken => !string.IsNullOrWhiteSpace(_token) && DateTime.UtcNow < _tokenValidUntilUtc;
        public bool IsReady => _isValidToken;

        private HttpClient _getClient
        {
            get
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrWhiteSpace(_options.PartnerId))
                {
                    client.DefaultRequestHeaders.Add("X-Personio-Partner-ID", _options.PartnerId);
                }
                if (!string.IsNullOrWhiteSpace(_options.AppId))
                {
                    client.DefaultRequestHeaders.Add("X-Personio-App-ID", _options.AppId);
                }

                if (!_isValidToken)
                {
                    _token = Auth();
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                return client;
            }
        }

        private HttpClient _postClient => _getClient;

        #endregion

        #region Constructor

        public PersonioClient(PersonioClientOptions options)
        {
            _options = options;
        }

        #endregion

        #region Auth

        public async Task<string> AuthAsync()
        {
            return await AuthAsync(_options.ClientId, _options.ClientSecret);
        }

        public async Task<string> AuthAsync(string? clientId, string? clientSecret)
        {
            var request = new AuthRequest() { ClientId = clientId, ClientSecret = clientSecret };
            return await AuthAsync(request);
        }

        /// <summary>
        /// https://developer.personio.de/reference/post_auth
        /// </summary>
        /// <returns>Authentication token response</returns>
        public async Task<string> AuthAsync(AuthRequest request)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var content = new StringContent(JsonConvert.SerializeObject(request));
            content.Headers.ContentType.MediaType = "application/json";

            var response = await client.PostAsync("https://api.personio.de/v1/auth", content);
            var result = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(result);
            _token = authResponse.Data?.Token;
            _tokenValidUntilUtc = DateTime.UtcNow.AddDays(1).AddMinutes(-1);
            return _token;
        }

        public string Auth()
        {
            return Auth(_options.ClientId, _options.ClientSecret);
        }

        public string Auth(string? clientId, string? clientSecret)
        {
            var request = new AuthRequest() { ClientId = clientId, ClientSecret = clientSecret };
            return Auth(request);
        }

        /// <summary>
        /// https://developer.personio.de/reference/post_auth
        /// </summary>
        /// <returns>Authentication token response</returns>
        public string Auth(AuthRequest request)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var content = new StringContent(JsonConvert.SerializeObject(request));
            content.Headers.ContentType.MediaType = "application/json";

            var response = client.PostAsync("https://api.personio.de/v1/auth", content).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(result);
            _token = authResponse.Data?.Token;
            _tokenValidUntilUtc = DateTime.UtcNow.AddDays(1).AddMinutes(-1);
            return _token;
        }

        #endregion

        #region Employees Async

        /// <summary>
        /// https://developer.personio.de/reference/get_company-employees
        /// </summary>
        /// <returns>List Company Employees</returns>
        public async Task<PagedListResponse<Employee>> GetEmployeesAsync(GetEmployeesRequest request)
        {
            var url = _getEmployeesUrl(request);
            return await _handlePagedListRequestAsync<Employee, GetEmployeesResponse>(url);
        }

        /// <summary>
        /// https://developer.personio.de/reference/get_company-employees
        /// </summary>
        /// <returns>List Company Employees</returns>
        public PagedListResponse<Employee> GetEmployees(GetEmployeesRequest request)
        {
            var url = _getEmployeesUrl(request);
            return _handlePagedListRequest<Employee, GetEmployeesResponse>(url);
        }

        private string _getEmployeesUrl(GetEmployeesRequest request)
        {
            var url = $"https://api.personio.de/v1/company/employees?limit={request.Limit}&offset={request.Offset}";
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                url += $"&email={UrlEncoder.Default.Encode(request.Email)}";
            }

            if (request.UpdatedSince.HasValue)
            {
                url += $"&updated_since={UrlEncoder.Default.Encode(request.UpdatedSince.Value.ToString(""))}";
            }

            if (request.Attributes != null && request.Attributes.Any())
            {
                foreach (var attribute in request.Attributes)
                {
                    url += $"&attributes[]={UrlEncoder.Default.Encode(attribute)}";
                }
            }
            return url;
        }

        /// <summary>
        /// https://developer.personio.de/reference/post_company-employees
        /// 
        /// Creates a new employee. If the employee's status is not provided, it will be set based 
        /// on the hire_date value - if it is in the past, status will be active, otherwise onboarding. 
        /// This endpoint responds with the id of the created employee in case of success.
        /// </summary>
        public async Task<CreateResponse> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request));
            var response = await _postClient.PostAsync("https://api.personio.de/v1/company/employees", content);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateResponse>(result);
        }

        /// <summary>
        /// https://developer.personio.de/reference/patch_company-employees-employee-id
        /// 
        /// Update existing employee. Note: Updating of Email field is not currently supported.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UpdateResponse> UpdateEmployeeAsync(UpdateEmployeeRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request));
            //var response = await _postClient.PatchAsync($"https://api.personio.de/v1/company/employees/{request.Id}", content);

            var message = new HttpRequestMessage(new HttpMethod("PATCH"), $"https://api.personio.de/v1/company/employees/{request.Id}");
            message.Content = content;
            var response = await _postClient.SendAsync(message);

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UpdateResponse>(result);
        }

        /// <summary>
        /// https://developer.personio.de/reference/get_company-employees-employee-id
        /// 
        /// Show employee by ID
        /// </summary>
        public async Task<Employee> GetEmployeeAsync(int id)
        {
            var result = await _getClient.GetStringAsync($"https://api.personio.de/v1/company/employees/{id}");
            return JsonConvert.DeserializeObject<Employee>(result);
        }

        /// <summary>
        /// https://developer.personio.de/reference/get_company-employees-employee-id-profile-picture-width
        /// 
        /// Show employee's profile picture. If profile picture is missing, the 404 error will be thrown. 
        /// The Profile Picture attribute has to be whitelisted.
        public async Task<byte[]> GetProfilePictureAsync(int id, int width)
        {
            var client = _getClient;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/png"));

            var response = await client.GetAsync($"https://api.personio.de/v1/company/employees/{id}/profile-picture/{width}");
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var mem = new MemoryStream())
                {
                    await stream.CopyToAsync(mem);
                    return mem.ToArray();
                }
            }
            return null;
        }

        #endregion

        #region Attendances

                /// <summary>
                /// https://developer.personio.de/reference/get_company-attendances
                /// 
                /// Fetch attendance data for the company employees. The result can be
                /// paginated and filtered by period, the date and/or time they were 
                /// updated, and/or specific employee/employees. The result 
                /// contains a list of attendances.
                /// </summary>
                public async Task GetAttendancesAsync(GetAttendencesRequest request)
                {
                    var url = $"https://api.personio.de/v1/company/attendances?start_date={request.StartDate.ToString(Constants.DATE_FORMAT)}&end_date={request.EndDate.ToString(Constants.DATE_FORMAT)}&limit={request.Limit}&offset={request.Offset}";
                    if (request.UpdatedFrom.HasValue)
                    {
                        url += $"updated_from={request.UpdatedFrom.Value.ToString(Constants.DATE_FORMAT)}";
                    }

                    if (request.UpdatedTo.HasValue)
                    {
                        url += $"updated_to={request.UpdatedTo.Value.ToString(Constants.DATE_FORMAT)}";
                    }

                    if (request.EmployeeIds != null && request.EmployeeIds.Any())
                    {
                        foreach (var employeeId in request.EmployeeIds)
                        {
                            url += $"&employees[]={employeeId}";
                        }
                    }

                    var result = await _getClient.GetStringAsync(url);
                }

                /// <summary>
                /// https://developer.personio.de/reference/post_company-attendances
                /// 
                /// This endpoint is responsible for adding attendance data for the 
                /// company employees. It is possible to add attendances for one or many 
                /// employees at the same time. The payload sent on the request should 
                /// be a list of attendance periods, in the form of an array containing 
                /// attendance period objects.
                /// </summary>
                public async Task<bool> CreateAttendancesAsync(AddAttendancesRequest request)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    var response = await _postClient.PostAsync("https://api.personio.de/v1/company/attendances", content);
                    var json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    var createResponse = JsonConvert.DeserializeObject<CreateResponse>(json);
                    if (!createResponse.Success)
                    {
                        createResponse.StatusCode = response.StatusCode;
                        return CreateResponse.FromError<IEnumerable<Attendance>>(createResponse);
                    }
                    return CreateResponse.FromData(createResponse.Data, response.StatusCode);
                }

                /// <summary>
                /// https://developer.personio.de/reference/delete_company-attendances-id
                /// 
                /// This endpoint is responsible for deleting attendance data for the company employees.
                /// </summary>
                public async Task<DeleteResponse> DeleteAttendancesAsync(int id, bool skipAPproval = true)
                {
                    var response = await _getClient.DeleteAsync($"https://api.personio.de/v1/company/attendances/{id}?skip_approval={skipAPproval}");
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DeleteResponse>(result);
                }

        #endregion

        #region Absences

        /// <summary>
        /// https://developer.personio.de/reference/get_company-time-off-types
        /// 
        /// Provides a list of absence types for absences tracked in days and 
        /// hours. For example 'Paid vacation', 'Parental leave' or 'Home office'.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedListResponse<TimeOffType>> GetTimeOffTypesAsync(GetTimeOffTypesRequest request)
        {
            var url = $"https://api.personio.de/v1/company/time-off-types?limit={request.Limit}&offset={request.Offset}";
            return await _handlePagedListRequestAsync<TimeOffType, GetTimeOffTypesResponse>(url);
        }

        /// <summary>
        /// https://developer.personio.de/reference/get_company-time-off-types
        /// 
        /// Provides a list of absence types for absences tracked in days and 
        /// hours. For example 'Paid vacation', 'Parental leave' or 'Home office'.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PagedListResponse<TimeOffType> GetTimeOffTypes(GetTimeOffTypesRequest request)
        {
            var url = $"https://api.personio.de/v1/company/time-off-types?limit={request.Limit}&offset={request.Offset}";
            return _handlePagedListRequest<TimeOffType, GetTimeOffTypesResponse>(url);
        }

        /// <summary>
        /// https://developer.personio.de/reference/get_company-time-offs
        /// 
        /// Fetches absence periods for absences tracked in days. The result 
        /// can be paginated and filtered by period and/or specific employee/employees.
        /// The result contains a list of absence periods.
        /// </summary>
        public async Task<PagedListResponse<TimeOffPeriod>> GetTimeOffsAsync(GetTimeOffsRequest request)
        {
            var url = _getTimeOffsUrl(request);
            return await _handlePagedListRequestAsync<TimeOffPeriod, GetTimeOffPeriodsResponse>(url);
        }

        /// <summary>
        /// https://developer.personio.de/reference/get_company-time-offs
        /// 
        /// Fetches absence periods for absences tracked in days. The result 
        /// can be paginated and filtered by period and/or specific employee/employees.
        /// The result contains a list of absence periods.
        /// </summary>
        public PagedListResponse<TimeOffPeriod> GetTimeOffs(GetTimeOffsRequest request)
        {
            var url = _getTimeOffsUrl(request);
            return _handlePagedListRequest<TimeOffPeriod, GetTimeOffPeriodsResponse>(url);
        }

        private string _getTimeOffsUrl(GetTimeOffsRequest request)
        {
            var url = $"https://api.personio.de/v1/company/time-offs?limit={request.Limit}&offset={request.Offset}";
            if (request.StartDate.HasValue)
            {
                url += $"&start_date={request.StartDate.Value.ToString(Constants.DATE_FORMAT)}";
            }

            if (request.EndDate.HasValue)
            {
                url += $"&end_date={request.EndDate.Value.ToString(Constants.DATE_FORMAT)}";
            }

            if (request.UpdatedFrom.HasValue)
            {
                url += $"&updated_from={request.UpdatedFrom.Value.ToString(Constants.DATE_FORMAT)}";
            }

            if (request.UpdatedTo.HasValue)
            {
                url += $"&updated_to={request.UpdatedTo.Value.ToString(Constants.DATE_FORMAT)}";
            }

            if (request.EmployeeIds != null && request.EmployeeIds.Any())
            {
                foreach (var employeeId in request.EmployeeIds)
                {
                    url += $"&employees[]={employeeId}";
                }
            }

            return url;
        }

        /// <summary>
        /// https://developer.personio.de/reference/post_company-time-offs
        /// 
        /// Adds absence data for absence types tracked in days.
        /// </summary>
        public async Task<CreateResponse<TimeOffPeriod>> CreateTimeOffAsync(CreateTimeOffRequest request)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "half_day_start", request.HalfDayStart.ToString().ToLower() },
                { "half_day_end", request.HalfDayEnd.ToString().ToLower() },
                { "employee_id", request.EmployeeId.ToString() },
                { "time_off_type_id", request.TimeOffTypeId.ToString() },
                { "start_date", request.StartDate.ToString(Constants.DATE_FORMAT) },
                { "end_date", request.EndDate.ToString(Constants.DATE_FORMAT) },
                { "comment", request.Comment },
                { "skip_approval", request.SkipApproval.ToString().ToLower() },
            });

            var response = await _postClient.PostAsync("https://api.personio.de/v1/company/time-offs", content);
            var json = await response.Content.ReadAsStringAsync();
            var createResponse = JsonConvert.DeserializeObject<CreateResponse<TypeAndAttributesObject<TimeOffPeriodAttributes>>>(json);
            if (!createResponse.Success)
            {
                createResponse.StatusCode = response.StatusCode;
                return CreateResponse.FromError<TimeOffPeriod>(createResponse);
            }
            return CreateResponse.FromData(createResponse.Data.Attributes.ToTimeOffPeriod(), response.StatusCode);
        }

        /// <summary>
        /// https://developer.personio.de/reference/post_company-time-offs
        /// 
        /// Adds absence data for absence types tracked in days.
        /// </summary>
        public CreateResponse<TimeOffPeriod> CreateTimeOff(CreateTimeOffRequest request)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "half_day_start", request.HalfDayStart.ToString().ToLower() },
                { "half_day_end", request.HalfDayEnd.ToString().ToLower() },
                { "employee_id", request.EmployeeId.ToString() },
                { "time_off_type_id", request.TimeOffTypeId.ToString() },
                { "start_date", request.StartDate.ToString(Constants.DATE_FORMAT) },
                { "end_date", request.EndDate.ToString(Constants.DATE_FORMAT) },
                { "comment", request.Comment },
                { "skip_approval", request.SkipApproval.ToString().ToLower() },
            });

            var response = _postClient.PostAsync("https://api.personio.de/v1/company/time-offs", content).ConfigureAwait(false).GetAwaiter().GetResult();
            var json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var createResponse = JsonConvert.DeserializeObject<CreateResponse<TypeAndAttributesObject<TimeOffPeriodAttributes>>>(json);
            if (!createResponse.Success)
            {
                createResponse.StatusCode = response.StatusCode;
                return CreateResponse.FromError<TimeOffPeriod>(createResponse);
            }
            return CreateResponse.FromData(createResponse.Data.Attributes.ToTimeOffPeriod(), response.StatusCode);
        }

        /// <summary>
        /// https://developer.personio.de/reference/delete_company-time-offs-id
        /// 
        /// Deletes absence period data for absence types tracked in days.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteTimeOffAsync(int id)
        {
            var response = await _getClient.DeleteAsync($"https://api.personio.de/v1/company/time-offs/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<DeleteResponse>(json);
            return result;
        }

        /// <summary>
        /// https://developer.personio.de/reference/delete_company-time-offs-id
        /// 
        /// Deletes absence period data for absence types tracked in days.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DeleteResponse DeleteTimeOff(int id)
        {
            var response = _getClient.DeleteAsync($"https://api.personio.de/v1/company/time-offs/{id}").ConfigureAwait(false).GetAwaiter().GetResult();
            var json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = JsonConvert.DeserializeObject<DeleteResponse>(json);
            result.StatusCode = response.StatusCode;
            return result;
        }

        /// <summary>
        /// https://developer.personio.de/reference/get_company-time-offs-id
        /// 
        /// Gets an absence period for absences tracked in days.
        /// </summary>
        public async Task<GetTimeOffResponse> GetTimeOffAsync(int id)
        {
            var response = await _getClient.GetAsync($"https://api.personio.de/v1/company/time-offs/{id}");
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetTimeOffResponse>(result);
        }

        /// <summary>
        /// https://developer.personio.de/reference/get_company-absence-periods
        /// 
        /// Fetches absence periods for absences tracked in hours. The result can be 
        /// paginated and filtered by period and/or specific employee/employees. 
        /// The result contains a list of hourly absence periods.
        /// </summary>
        public async Task GetAbsencePeriodsAsync(GetAbsencePeriodsRequest request)
        {
            var url = $"https://api.personio.de/v1/company/absence-periods" +
                $"?start_date={request.StartDate.ToString(Constants.DATE_FORMAT)}" +
                $"&end_date={request.EndDate.ToString(Constants.DATE_FORMAT)}" +
                $"&updated_from={UrlEncoder.Default.Encode(request.UpdatedFrom.ToString(Constants.DATE_TIME_FORMAT))}" +
                $"&updated_to={UrlEncoder.Default.Encode(request.UpdatedTo.ToString(Constants.DATE_TIME_FORMAT))}" +
                $"&limit=1&offset=1";

            if (request.EmployeeIds != null && request.EmployeeIds.Any())
            {
                foreach (var employeeId in request.EmployeeIds)
                {
                    url += $"&employees[]={employeeId}";
                }
            }

            if (request.AbsenceTypes != null && request.AbsenceTypes.Any())
            {
                foreach (var absenceType in request.AbsenceTypes)
                {
                    url += $"&absence_types[]={absenceType}";
                }
            }

            if (request.AbsencePeriods != null && request.AbsencePeriods.Any())
            {
                foreach (var absencePeriod in request.AbsencePeriods)
                {
                    url += $"&absence_periods[]={absencePeriod}";
                }
            }

            var response = await _getClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
#warning TODO result anschauen
        }

        /// <summary>
        /// https://developer.personio.de/reference/post_company-absence-periods
        /// 
        /// Adds absence data for absence types tracked in hours. Note that creating 
        /// periods for absence types with certificate requirement enabled is not supported!
        /// </summary>
        public async Task CreateAbsencePeriod(CreateAbsencePeriodRequest request)
        {
            var parameters = new Dictionary<string, string>
            {
                { "employee_id", request.EmployeeId.ToString() },
                { "time_off_type_id", request.TimeOffTypeId.ToString() },
                { "start_date", request.StartDate.ToString(Constants.DATE_FORMAT) },
                { "end_date", request.EndDate.ToString(Constants.DATE_FORMAT)  },
                { "half_day_start", request.HalfDayStart.ToString().ToLower() },
                { "half_day_end", request.HalfDayEnd.ToString().ToLower() },
                { "comment", request.Comment },
                { "skip_approval", request.SkipApproval.ToString().ToLower() },
                { "start_time", request.StartTime.ToString(Constants.TIME_FORMAT) },
                { "end_time", request.EndTime.ToString(Constants.TIME_FORMAT) },
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _postClient.PostAsync("https://api.personio.de/v1/company/absence-periods", content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
#warning TODO result anschauen
        }

        /// <summary>
        /// https://developer.personio.de/reference/delete_company-absence-periods-id
        /// 
        /// Deletes absence period data for absence types tracked in hours.
        /// </summary>
        public async Task<DeleteResponse> DeleteAbsencePeriodAsync(string id)
        {
            var response = await _getClient.DeleteAsync($"https://api.personio.de/v1/company/absence-periods/{id}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeleteResponse>(result);
        }

        #endregion

        #region Helpers

        private PagedListResponse<TItem> _handlePagedListRequest<TItem, TResponse>(string url)
            where TResponse : BasePagedListResponse<TItem>
        {
            var response = _getClient.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
            var responseContent = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var errorData = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return new PagedListResponse<TItem>()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    Error = errorData.Error
                };
            }
            try
            {
                var data = JsonConvert.DeserializeObject<TResponse>(responseContent, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                return new PagedListResponse<TItem>()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    PagedList = data.ToPagedList()
                };
            }
            catch (Exception e)
            {
                return new PagedListResponse<TItem>()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    Error = new Error()
                    {
                        Message = e.Message,
                        ErrorData = new Dictionary<string, object>()
                        {
                            { "StackTrace", e.StackTrace }
                        }
                    },
                    Raw = responseContent
                };
            }
        }

        private async Task<PagedListResponse<TItem>> _handlePagedListRequestAsync<TItem, TResponse>(string url)
            where TResponse : BasePagedListResponse<TItem>
        {
            var response = await _getClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorData = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return new PagedListResponse<TItem>()
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    Error = errorData.Error
                };
            }
            var data = JsonConvert.DeserializeObject<TResponse>(responseContent);
            return new PagedListResponse<TItem>()
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = response.ReasonPhrase,
                PagedList = data.ToPagedList()
            };
        }

        #endregion
    }
}