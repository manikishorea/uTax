Module modMain

    Sub Main()

        MyConfig.Setup()
        DBAccessDest.Init(MyConfig.DestDBServerName, MyConfig.DestDBName, MyConfig.DestDBUsername, MyConfig.DestDBPassword, MyConfig.DestDBTrustedConnection)
        DBAccessSource.Init(MyConfig.SourceDBServerName, MyConfig.SourceDBName, MyConfig.SourceDBUsername, MyConfig.SourceDBPassword, MyConfig.SourceDBTrustedConnection)

        IdentifySalesforceClosedCases()
        SyncEMPdatatoSFDC()

        Console.WriteLine("update done")
        'Console.ReadLine()

    End Sub

    Private Sub IdentifySalesforceClosedCases()
        Try
            Dim strQ As String = "select top 1 * from SalesYearMaster where ApplicableToDate is null order by SalesYear desc"
            Dim ds As DataSet = DBAccessDest.ExecuteQuery(strQ)
            Dim ActiveSalesYear As String = ""
            If ds.Tables(0).Rows.Count > 0 Then
                ActiveSalesYear = CStr(ds.Tables(0).Rows(0).Item("SalesYear")).ToString()
            End If
            'Console.WriteLine(ActiveSalesYear)
            If ActiveSalesYear <> "" Then
                strQ = "Select * from Opportunity where Opportunity.RecordTypeId = '012E00000002LFQIA2' and StageName = 'Closed Won' and Tax_Season__c = '" & ActiveSalesYear & "'"
                ds = DBAccessSource.ExecuteQuery(strQ)
                If ds.Tables.Count > 0 Then
                    If ds.Tables(0).Rows.Count > 0 Then
                        For Each drO As DataRow In ds.Tables(0).Rows
                            Dim SalesforceOppurtunityID As String = CStr(drO.Item("ID"))
                            Dim SalesforceAccountID As String = CStr(drO.Item("AccountID"))
                            Try
                                Dim dsTemp As DataSet = New DataSet()

                                Dim uTax_Not_Collecting_SB_Fees__c As String = CStr(drO.Item("uTax_Not_Collecting_SB_Fees__c"))
                                Dim buTax_Not_Collecting_SB_Fees__c As Boolean = False

                                If uTax_Not_Collecting_SB_Fees__c.ToUpper.Trim = "TRUE" Then
                                    buTax_Not_Collecting_SB_Fees__c = True
                                End If
                                Dim OpportunityOwnerID As String = CStr(drO.Item("OwnerId"))
                                'Dim AccountOwnerID As String = CStr(drO.Item("AccountOwnerID"))

                                Dim OpportunityType As String = ""
                                If IsDBNull(drO.Item("Type")) Then
                                    OpportunityType = ""
                                Else
                                    OpportunityType = CStr(drO.Item("Type"))
                                End If

                                Dim Cash_Saver__c As String = ""
                                If IsDBNull(drO.Item("Cash_Saver__c")) Then
                                    Cash_Saver__c = ""
                                Else
                                    Cash_Saver__c = CStr(drO.Item("Cash_Saver__c"))
                                End If

                                Dim pymt__Balance__c As String = ""
                                If IsDBNull(drO.Item("pymt__Balance__c")) Then
                                    pymt__Balance__c = 0
                                Else
                                    pymt__Balance__c = CStr(drO.Item("pymt__Balance__c"))
                                End If

                                Dim LOC_Program_Participant__c As String = ""
                                If IsDBNull(drO.Item("LOC_Program_Participant__c")) Then
                                    LOC_Program_Participant__c = ""
                                Else
                                    LOC_Program_Participant__c = CStr(drO.Item("LOC_Program_Participant__c"))
                                End If

                                Dim Total_Amount_Loaned__c As String = ""
                                If IsDBNull(drO.Item("Total_Amount_Loaned__c")) Then
                                    Total_Amount_Loaned__c = 0
                                Else
                                    Total_Amount_Loaned__c = CStr(drO.Item("Total_Amount_Loaned__c"))
                                End If

                                Dim A_R_Amount_Due_Credit__c As String = ""
                                If IsDBNull(drO.Item("A_R_Amount_Due_Credit__c")) Then
                                    A_R_Amount_Due_Credit__c = 0
                                Else
                                    A_R_Amount_Due_Credit__c = CStr(drO.Item("A_R_Amount_Due_Credit__c"))
                                End If

                                Dim Federal_EF_Fee_New__c As String = ""
                                If IsDBNull(drO.Item("Federal_EF_Fee_New__c")) Then
                                    Federal_EF_Fee_New__c = 0
                                Else
                                    Federal_EF_Fee_New__c = CStr(drO.Item("Federal_EF_Fee_New__c"))
                                End If

                                Dim State_EF_Fee_New__c As String = ""
                                If IsDBNull(drO.Item("State_EF_Fee_New__c")) Then
                                    State_EF_Fee_New__c = 0
                                Else
                                    State_EF_Fee_New__c = CStr(drO.Item("State_EF_Fee_New__c"))
                                End If

                                Dim Quote_Software_Package__c As String = ""
                                If IsDBNull(drO.Item("Quote_Software_Package__c")) Then
                                    Quote_Software_Package__c = ""
                                Else
                                    Quote_Software_Package__c = CStr(drO.Item("Quote_Software_Package__c"))
                                End If

                                Dim Transmitter_Fee__c As Double = 0
                                If IsDBNull(drO.Item("Transmitter_Fee__c")) Then
                                    Transmitter_Fee__c = 0
                                Else
                                    Transmitter_Fee__c = CDbl(drO.Item("Transmitter_Fee__c"))
                                End If

                                Dim Technology_Fee__c As Double = 0
                                If IsDBNull(drO.Item("Technology_Fee__c")) Then
                                    Technology_Fee__c = 0
                                Else
                                    Technology_Fee__c = CDbl(drO.Item("Technology_Fee__c"))
                                End If

                                Dim Quote_Bank_Product_uTax_Fee__c As Double = 0
                                If IsDBNull(drO.Item("Quote_Bank_Product_uTax_Fee__c")) Then
                                    Quote_Bank_Product_uTax_Fee__c = 0
                                Else
                                    Quote_Bank_Product_uTax_Fee__c = CDbl(drO.Item("Quote_Bank_Product_uTax_Fee__c"))
                                End If

                                Dim Quote_Federal_EF_Fee__c As Double = 0
                                If IsDBNull(drO.Item("Quote_Federal_EF_Fee__c")) Then
                                    Quote_Federal_EF_Fee__c = 0
                                Else
                                    Quote_Federal_EF_Fee__c = CDbl(drO.Item("Quote_Federal_EF_Fee__c"))
                                End If

                                Dim Quote_State_EF_Fee__c As Double = 0
                                If IsDBNull(drO.Item("Quote_State_EF_Fee__c")) Then
                                    Quote_State_EF_Fee__c = 0
                                Else
                                    Quote_State_EF_Fee__c = CDbl(drO.Item("Quote_State_EF_Fee__c"))
                                End If

                                Dim Quote_Rebate__c As Boolean = False
                                If IsDBNull(drO.Item("Quote_Rebate__c")) Then
                                    Quote_Rebate__c = False
                                Else
                                    Quote_Rebate__c = CBool(drO.Item("Quote_Rebate__c"))
                                End If

                                Dim SalesforceLastActivityDate As Date = CDate("1900-01-01 00:00:00")
                                If IsDBNull(drO.Item("LastActivityDate")) Then
                                    SalesforceLastActivityDate = CDate("1900-01-01 00:00:00")
                                Else
                                    SalesforceLastActivityDate = CDate(drO.Item("LastActivityDate"))
                                End If

                                Dim SalesforceLastModifiedDate As Date = CDate("1900-01-01 00:00:00")
                                If IsDBNull(drO.Item("LastModifiedDate")) Then
                                    SalesforceLastModifiedDate = CDate("1900-01-01 00:00:00")
                                Else
                                    SalesforceLastModifiedDate = CDate(drO.Item("LastModifiedDate"))
                                End If

                                strQ = "Select * from Account where ID = '" & SalesforceAccountID & "'"
                                Dim dsAcc As DataSet = DBAccessSource.ExecuteQuery(strQ)

                                If dsAcc.Tables.Count > 0 Then
                                    If dsAcc.Tables(0).Rows.Count > 0 Then

                                        Dim CompanyName As String = ""
                                        Dim Feeder As Boolean = False
                                        Dim EROType As String = ""
                                        Dim EFIN As Integer = 0
                                        Dim EntityTypeID As Integer = 0
                                        Dim masterIdentifier As String = ""
                                        Dim SalesYearID As String = ""
                                        Dim PhysicalAddress1 As String = ""
                                        Dim PhysicalZipCode As String = ""
                                        Dim PhysicalCity As String = ""
                                        Dim PhysicalState As String = ""
                                        Dim ShippingAddress1 As String = ""
                                        Dim ShippingZipCode As String = ""
                                        Dim ShippingCity As String = ""
                                        Dim ShippingState As String = ""
                                        Dim OfficePhone As String = ""
                                        Dim PrimaryEmail As String = ""
                                        Dim SFParentID As String = ""
                                        Dim EMPParentID As String = ""
                                        Dim TitleId As String = ""
                                        Dim Title As String = ""

                                        Dim BusinessOwnerFName As String = ""
                                        Dim BusinessOwnerLName As String = ""
                                        Dim Transmission_Password__c As String = ""

                                        Dim AccountLastActivityDate As Date = CDate("1900-01-01 00:00:00")
                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("LastActivityDate")) Then
                                            AccountLastActivityDate = CDate("1900-01-01 00:00:00")
                                        Else
                                            AccountLastActivityDate = CDate(dsAcc.Tables(0).Rows(0).Item("LastActivityDate"))
                                        End If

                                        Dim AccountLastModifiedDate As Date = CDate("1900-01-01 00:00:00")
                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("LastModifiedDate")) Then
                                            AccountLastModifiedDate = CDate("1900-01-01 00:00:00")
                                        Else
                                            AccountLastModifiedDate = CDate(dsAcc.Tables(0).Rows(0).Item("LastModifiedDate"))
                                        End If

                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("Name")) Then
                                            CompanyName = ""
                                        Else
                                            CompanyName = CStr(dsAcc.Tables(0).Rows(0).Item("Name")).Trim("'")
                                        End If

                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("Master_Identifier__c")) Then
                                            masterIdentifier = ""
                                        Else
                                            masterIdentifier = CStr(dsAcc.Tables(0).Rows(0).Item("Master_Identifier__c")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("EFIN__c")) Then
                                            EFIN = CStr(dsAcc.Tables(0).Rows(0).Item("EFIN__c").ToString().Trim("'"))
                                        Else
                                            EFIN = 0
                                        End If

                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("Transmission_Password__c")) Then
                                            Transmission_Password__c = ""
                                        Else
                                            Transmission_Password__c = CStr(dsAcc.Tables(0).Rows(0).Item("Transmission_Password__c")).Trim("'")
                                        End If

                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("ParentID")) Then
                                            SFParentID = ""
                                        Else
                                            SFParentID = CStr(dsAcc.Tables(0).Rows(0).Item("ParentID")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("Feeder_Sites__c")) Then
                                            If CStr(dsAcc.Tables(0).Rows(0).Item("Feeder_Sites__c")).ToLower = "true" Then
                                                Feeder = True
                                            End If
                                        End If

                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("ERO_Type__c")) Then
                                            EROType = ""
                                        Else
                                            EROType = CStr(dsAcc.Tables(0).Rows(0).Item("ERO_Type__c")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("BillingStreet")) Then
                                            PhysicalAddress1 = CStr(dsAcc.Tables(0).Rows(0).Item("BillingStreet")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("BillingCity")) Then
                                            PhysicalCity = CStr(dsAcc.Tables(0).Rows(0).Item("BillingCity")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("BillingState")) Then
                                            PhysicalState = CStr(dsAcc.Tables(0).Rows(0).Item("BillingState")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("BillingPostalCode")) Then
                                            PhysicalZipCode = CStr(dsAcc.Tables(0).Rows(0).Item("BillingPostalCode")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("ShippingStreet")) Then
                                            ShippingAddress1 = CStr(dsAcc.Tables(0).Rows(0).Item("ShippingStreet")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("ShippingCity")) Then
                                            ShippingCity = CStr(dsAcc.Tables(0).Rows(0).Item("ShippingCity")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("ShippingState")) Then
                                            ShippingState = CStr(dsAcc.Tables(0).Rows(0).Item("ShippingState")).Trim("'")
                                        End If

                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("ShippingPostalCode")) Then
                                            ShippingZipCode = CStr(dsAcc.Tables(0).Rows(0).Item("ShippingPostalCode")).Trim("'")
                                        End If

                                        Dim MSO_User__c As String = ""
                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("MSO_User__c")) Then
                                            MSO_User__c = CStr(dsAcc.Tables(0).Rows(0).Item("MSO_User__c"))
                                        End If
                                        Dim bMSO_User__c As Boolean = False
                                        If MSO_User__c.ToUpper.Trim = "TRUE" Then
                                            bMSO_User__c = True
                                        End If

                                        Dim Co_Branding__c As String = ""
                                        If Not IsDBNull(dsAcc.Tables(0).Rows(0).Item("Co_Branding__c")) Then
                                            Co_Branding__c = CStr(dsAcc.Tables(0).Rows(0).Item("Co_Branding__c"))
                                        End If
                                        Dim bCo_Branding__c As Boolean = False
                                        If Co_Branding__c.ToUpper.Trim = "TRUE" Then
                                            bCo_Branding__c = True
                                        End If

                                        Dim User_ID__c As String = ""
                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("User_ID__c")) Then
                                            User_ID__c = ""
                                        Else
                                            User_ID__c = CStr(dsAcc.Tables(0).Rows(0).Item("User_ID__c")).Trim("'")
                                        End If

                                        strQ = "Select * from EntityMaster where [Description] = '" & EROType & "'"
                                        dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                        Dim IsAdditionalEFINAllowed As Boolean = False
                                        If dsTemp.Tables(0).Rows.Count > 0 Then
                                            EntityTypeID = CType(dsTemp.Tables(0).Rows(0).Item("Id"), Integer)
                                            IsAdditionalEFINAllowed = CType(dsTemp.Tables(0).Rows(0).Item("IsAdditionalEFINAllowed"), Boolean)
                                        End If

                                        If EntityTypeID = 0 Then
                                            EmailManager.SendEmail("Customer Not Created", "Invalid ERO Type for Company " & CompanyName)
                                            Continue For
                                        End If

                                        strQ = "Select * from SalesYearMaster where [SalesYear] = '" & ActiveSalesYear & "'"
                                        dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                        If dsTemp.Tables(0).Rows.Count > 0 Then
                                            SalesYearID = CType(dsTemp.Tables(0).Rows(0).Item("Id"), System.Guid).ToString()
                                        End If

                                        Dim EFIN_Status__c As String = ""
                                        If IsDBNull(dsAcc.Tables(0).Rows(0).Item("EFIN_Status__c")) Then
                                            EFIN_Status__c = ""
                                        Else
                                            EFIN_Status__c = CStr(dsAcc.Tables(0).Rows(0).Item("EFIN_Status__c"))
                                        End If
                                        Dim EFINStatusId As Integer = 0
                                        If EFIN_Status__c = "Active" Then
                                            EFIN_Status__c = "Valid EFIN"
                                        End If

                                        Dim RefTypeId As Integer = 4
                                        If EntityTypeID = 10 OrElse EntityTypeID = 6 OrElse EntityTypeID = 11 Then
                                            RefTypeId = 5
                                        End If

                                        strQ = "Select * from StatusCode where [DisplayText] = '" & EFIN_Status__c & "' and RefTypeId=" & RefTypeId & ""
                                        ds = DBAccessDest.ExecuteQuery(strQ)
                                        If ds.Tables(0).Rows.Count > 0 Then
                                            EFINStatusId = CType(ds.Tables(0).Rows(0).Item("Id"), Integer)
                                        End If

                                        If Not SFParentID = "" Then
                                            strQ = "Select * from emp_CustomerInformation where SalesforceAccountID = '" & SFParentID & "'"
                                            dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                            If dsTemp.Tables(0).Rows.Count > 0 Then
                                                EMPParentID = CType(dsTemp.Tables(0).Rows(0).Item("Id"), Guid).ToString()
                                            End If
                                        End If

                                        If SFParentID <> "" And EMPParentID = "" Then
                                            EmailManager.SendEmail("Customer Not Created", "Subsite doesn't have Parent Info for Company : " & CompanyName)
                                            Continue For
                                        End If

                                        If EMPParentID = "" Then
                                            EMPParentID = "NULL"
                                        Else
                                            EMPParentID = "'" & EMPParentID & "'"
                                        End If

                                        If EMPParentID <> "NULL" And (EntityTypeID = 2 OrElse EntityTypeID = 3 OrElse EntityTypeID = 5 Or EntityTypeID = 9) Then
                                            EMPParentID = "NULL"
                                        End If

                                        strQ = "Select * from Contact where Primary_Contact__c = 'true' and AccountId = '" & SalesforceAccountID & "'"
                                        ds = DBAccessSource.ExecuteQuery(strQ)
                                        If ds.Tables(0).Rows.Count > 0 Then
                                            If Not IsDBNull(ds.Tables(0).Rows(0).Item("FirstName")) Then
                                                BusinessOwnerFName = CStr(ds.Tables(0).Rows(0).Item("FirstName")).Trim("'") '& " " & CStr(ds.Tables(0).Rows(0).Item("LastName"))
                                            End If

                                            If Not IsDBNull(ds.Tables(0).Rows(0).Item("LastName")) Then
                                                BusinessOwnerLName = CStr(ds.Tables(0).Rows(0).Item("LastName")).Trim("'")
                                            End If

                                            If Not IsDBNull(ds.Tables(0).Rows(0).Item("Phone")) Then
                                                OfficePhone = CStr(ds.Tables(0).Rows(0).Item("Phone")).Trim("'")
                                            End If

                                            If Not IsDBNull(ds.Tables(0).Rows(0).Item("Email")) Then
                                                PrimaryEmail = CStr(ds.Tables(0).Rows(0).Item("Email")).Trim("'")
                                            End If

                                            If Not IsDBNull(ds.Tables(0).Rows(0).Item("Title")) Then
                                                Title = CStr(ds.Tables(0).Rows(0).Item("Title")).Trim("'")
                                            End If
                                        End If

                                        If Title <> "" Then
                                            strQ = "Select * from ContactPersonTitleMaster where ContactPersonTitle = '" & Title & "'"
                                            dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                            If dsTemp.Tables.Count > 0 Then
                                                If dsTemp.Tables(0).Rows.Count > 0 Then
                                                    TitleId = CType(dsTemp.Tables(0).Rows(0).Item("Id"), Guid).ToString()
                                                End If
                                            End If
                                        End If

                                        If TitleId = "" Then
                                            TitleId = "NULL"
                                        Else
                                            TitleId = "'" & TitleId & "'"
                                        End If

                                        'Console.WriteLine(CompanyName)

                                        strQ = "select * from emp_CustomerInformation where SalesforceAccountID = '" & SalesforceAccountID & "' and SalesforceOpportunityID='" & SalesforceOppurtunityID & "'"
                                        dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                        If dsTemp.Tables.Count > 0 Then
                                            If dsTemp.Tables(0).Rows.Count = 0 Then
                                                'need to insert
#Region "inserting new record"
                                                'Console.WriteLine("insert" & CompanyName)
                                                strQ = "Insert into emp_CustomerInformation (IsAdditionalEFINAllowed, CompanyName, EntityId, Feeder, BusinessOwnerFirstName, BusinesOwnerLastName, OfficePhone, AlternatePhone, " &
                                                        "PrimaryEmail, SupportNotificationEmail, EROType, AlternativeContact, EFIN, EFINStatus, PhysicalAddress1, PhysicalAddress2, PhysicalZipCode, PhysicalCity, " &
                                                        "PhysicalState, ShippingAddressSameAsPhysicalAddress, ShippingAddress1, ShippingAddress2, ShippingZipCode, ShippingCity, ShippingState, StatusCode, " &
                                                        "SalesforceAccountID, SalesforceOpportunityID, SalesforceParentID, ParentID, MSOUser,IsMSOUser, uTaxNotCollectingSBFee, Cash_Saver__c, pymt__Balance__c, " &
                                                        "LOC_Program_Participant__c, Total_Amount_Loaned__c, A_R_Amount_Due_Credit__c, Federal_EF_Fee_New__c, State_EF_Fee_New__c, SalesYearId,TitleId,AccountStatus,QuoteSoftwarePackage,Quote_Rebate__c) values (" &
                                                        "'" & IsAdditionalEFINAllowed & "', '" & CompanyName.Replace("'", "''") & "', " & EntityTypeID & ", '" & Feeder & "', '" & BusinessOwnerFName.Replace("'", "''") & "', '" & BusinessOwnerLName.Replace("'", "''") & "', '" & OfficePhone.Replace("'", "''") & "', '', '" & PrimaryEmail & "', '', '" & EROType & "', '', " &
                                                        EFIN & ", " & EFINStatusId & ", '" & PhysicalAddress1.Replace("'", "''") & "', '', '" & PhysicalZipCode & "', '" & PhysicalCity.Replace("'", "''") & "', '" & PhysicalState & "', 0, '" & ShippingAddress1.Replace("'", "''") & "', '', '" &
                                                        ShippingZipCode & "', '" & ShippingCity & "', '" & ShippingState & "', 'NEW', '" & SalesforceAccountID & "', '" & SalesforceOppurtunityID & "','" & SFParentID & "'," & EMPParentID & ", " & IIf(bMSO_User__c, 1, 0) & ", " & IIf(bMSO_User__c, 1, 0) & ", " & IIf(buTax_Not_Collecting_SB_Fees__c, 1, 0) & ", '" & Cash_Saver__c & "', " & pymt__Balance__c & ", " &
                                                        "'" & LOC_Program_Participant__c & "', " & Total_Amount_Loaned__c & ", " & A_R_Amount_Due_Credit__c & ", " & Quote_Federal_EF_Fee__c & ", " & Quote_State_EF_Fee__c & ", '" & SalesYearID & "'," & TitleId & ",'Not Active','" & Quote_Software_Package__c & "'," & Quote_Rebate__c & " )"

                                                DBAccessDest.ExecuteQuery(strQ)

                                                Dim NewCustomerID As String = ""
                                                strQ = "Select * from emp_CustomerInformation where SalesforceOpportunityID = '" & SalesforceOppurtunityID & "' and SalesforceOpportunityID='" & SalesforceOppurtunityID & "'"
                                                dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                                If dsTemp.Tables(0).Rows.Count > 0 Then
                                                    NewCustomerID = CType(dsTemp.Tables(0).Rows(0).Item("Id"), System.Guid).ToString()
                                                End If

                                                If NewCustomerID <> "" Then

                                                    Dim EncryptedTransPassword As String = ""
                                                    If Transmission_Password__c <> "" Then
                                                        EncryptedTransPassword = PasswordManager.CryptText(Transmission_Password__c)
                                                    End If

                                                    strQ = "Insert into emp_CustomerLoginInformation (CustomerOfficeID, MasterIdentifier, OfficePortalURL, " &
                                                           " CrossLinkUserId, CrossLinkPassword, EMPUserId, EMPPassword,StatusCode) values ('" &
                                                           NewCustomerID & "', '" & masterIdentifier & "','https://www.mytaxofficeportal.com', " &
                                                           "'" & User_ID__c & "', '" & EncryptedTransPassword & "', '" & User_ID__c & "', '" & EncryptedTransPassword & "','ACT' )"
                                                    DBAccessDest.ExecuteQuery(strQ)

                                                    strQ = "Select * from salesforce_sync_summary where salesforceOpportunityID = '" & SalesforceOppurtunityID & "' and SalesYear = '" & ActiveSalesYear & "'"
                                                    dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                                    If dsTemp.Tables(0).Rows.Count > 0 Then
                                                        strQ = "update salesforce_sync_summary set Lastsync_timestamp=GETDATE() where salesforceOpportunityID = '" & SalesforceOppurtunityID & "' and SalesYear = '" & ActiveSalesYear & "'"
                                                    Else
                                                        strQ = "insert into salesforce_sync_summary (sync_timestamp,salesforceOpportunityID,salesforceAccountID,EROType,OpportunityLastActivityDate,OpportunityLastModifiedDate,AccountLastActivityDate1,AccountLastModifiedDate1,emp_CustomerID,SalesYear,Lastsync_timestamp)
                                                         values (GETDATE(),'" & SalesforceOppurtunityID & "','" & SalesforceAccountID & "','" & EROType & "','" & SalesforceLastActivityDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & SalesforceLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & AccountLastActivityDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & AccountLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & NewCustomerID & "','" & ActiveSalesYear & "',GETDATE())"
                                                    End If
                                                    DBAccessDest.ExecuteQuery(strQ)

                                                    'DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                    If User_ID__c <> "" And Transmission_Password__c <> "" Then
                                                        strQ = "Update emp_CustomerInformation set StatusCode = 'CRT' where ID = '" & NewCustomerID & "'"
                                                        DBAccessDest.ExecuteQuery(strQ)

                                                        'DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                        Select Case EntityTypeID
                                                            Case 2 'SO 
                                                                strQ = "Update emp_CustomerInformation set IsActivationCompleted = 0, IsVerified = 0, AccountStatus = 'Not Active', StatusCode = 'CRT' where ID = '" & NewCustomerID & "'"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                'strQ = "Update OfficeManagement set SalesYearID = '" & SalesYearID & "', CanEnrollmentAllowed = 0, CanEnrollmentAllowedForMain = 0, EFINStatus =" & EFINStatusId & "  where CustomerID = '" & NewCustomerID & "'"
                                                                'DBAccessDest.ExecuteQuery(strQ)

                                                                'DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 2, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                            Case 3 'SOME  
                                                                strQ = "Update emp_CustomerInformation set IsActivationCompleted = 0, IsVerified = 0, AccountStatus = 'Not Active', StatusCode = 'CRT' where ID = '" & NewCustomerID & "'"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                'strQ = "Update OfficeManagement set SalesYearID = '" & SalesYearID & "', CanEnrollmentAllowed = 0, CanEnrollmentAllowedForMain = 0, EFINStatus =" & EFINStatusId & "  where CustomerID = '" & NewCustomerID & "'"
                                                                'DBAccessDest.ExecuteQuery(strQ)

                                                                'DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 3, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                            Case 5 'MO Main 
                                                                strQ = "Update emp_CustomerInformation set IsActivationCompleted = 0, IsVerified = 0, AccountStatus = 'Not Active', StatusCode = 'CRT' where ID = '" & NewCustomerID & "'"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                'strQ = "Update OfficeManagement set SalesYearID = '" & SalesYearID & "', CanEnrollmentAllowed = 0, CanEnrollmentAllowedForMain = 0, EFINStatus =" & EFINStatusId & "  where CustomerID = '" & NewCustomerID & "'"
                                                                'DBAccessDest.ExecuteQuery(strQ)

                                                                'DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 5, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)


                                                            Case 9 'SVB Main 
                                                                strQ = "Update emp_CustomerInformation set IsActivationCompleted = 0, IsVerified = 0, AccountStatus = 'Not Active', StatusCode = 'CRT' where ID = '" & NewCustomerID & "'"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                'strQ = "Update OfficeManagement set SalesYearID = '" & SalesYearID & "', CanEnrollmentAllowed = 0, CanEnrollmentAllowedForMain = 0, EFINStatus =" & EFINStatusId & "  where CustomerID = '" & NewCustomerID & "'"
                                                                'DBAccessDest.ExecuteQuery(strQ)

                                                                'DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 9, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                            Case 6 'MO-SO 
                                                                strQ = "Update emp_CustomerInformation set IsActivationCompleted = 0, IsVerified = 0, AccountStatus = 'Not Active', StatusCode = 'CRT' where ID = '" & NewCustomerID & "'"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                'strQ = "Update OfficeManagement set SalesYearID = '" & SalesYearID & "', CanEnrollmentAllowed = 0, CanEnrollmentAllowedForMain = 0, EFINStatus =" & EFINStatusId & "  where CustomerID = '" & NewCustomerID & "'"
                                                                'DBAccessDest.ExecuteQuery(strQ)

                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 6, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                Dim rootparentid As String = GetRootParent(EMPParentID, NewCustomerID)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID, rootparentid)

                                                            Case 10 'SVB-SO 
                                                                strQ = "Update emp_CustomerInformation set IsActivationCompleted = 0, IsVerified = 0, AccountStatus = 'Not Active', StatusCode = 'CRT' where ID = '" & NewCustomerID & "'"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                'strQ = "Update OfficeManagement set SalesYearID = '" & SalesYearID & "', CanEnrollmentAllowed = 0, CanEnrollmentAllowedForMain = 0, EFINStatus =" & EFINStatusId & "  where CustomerID = '" & NewCustomerID & "'"
                                                                'DBAccessDest.ExecuteQuery(strQ)

                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 10, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                Dim rootparentid As String = GetRootParent(EMPParentID, NewCustomerID)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID, rootparentid)

                                                            Case 11 'SVB-MO 
                                                                strQ = "Update emp_CustomerInformation set IsActivationCompleted = 0, IsVerified = 0, AccountStatus = 'Not Active', StatusCode = 'CRT' where ID = '" & NewCustomerID & "'"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                'strQ = "Update OfficeManagement set SalesYearID = '" & SalesYearID & "', CanEnrollmentAllowed = 0, CanEnrollmentAllowedForMain = 0, EFINStatus =" & EFINStatusId & "  where CustomerID = '" & NewCustomerID & "'"
                                                                'DBAccessDest.ExecuteQuery(strQ)

                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 11, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                Dim rootparentid As String = GetRootParent(EMPParentID, NewCustomerID)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID, rootparentid)

                                                        End Select
                                                    Else
                                                        Select Case EntityTypeID
                                                            Case 2 'SO 
                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 2, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                            Case 3 'SOME  
                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 3, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                            Case 5 'MO Main 
                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 5, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                            Case 9 'SVB Main 
                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 9, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID)

                                                            Case 6 'MO-SO 
                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 6, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)
                                                                Dim rootparentid As String = GetRootParent(EMPParentID, NewCustomerID)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID, rootparentid)

                                                            Case 10 'SVB-SO 
                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 10, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)
                                                                Dim rootparentid As String = GetRootParent(EMPParentID, NewCustomerID)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID, rootparentid)

                                                            Case 11 'SVB-MO 
                                                                strQ = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                      NewCustomerID & "', 0, '" & NewCustomerID & "', 11, 'ACT')"
                                                                DBAccessDest.ExecuteQuery(strQ)

                                                                Dim rootparentid As String = GetRootParent(EMPParentID, NewCustomerID)
                                                                DBAccessDest.OfficeManagementGridSP(NewCustomerID, SalesYearID, rootparentid)
                                                        End Select
                                                    End If
                                                End If
#End Region
                                            Else
                                                'updating the emp_customerinformation data
#Region "Updateing"
                                                Dim canUpdate As Boolean = False
                                                'Console.WriteLine("Update" & CompanyName)
                                                strQ = "Select * from salesforce_sync_summary where salesforceOpportunityID = '" & SalesforceOppurtunityID & "' and SalesYear = '" & ActiveSalesYear & "'"
                                                dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                                If dsTemp.Tables(0).Rows.Count > 0 Then
                                                    Dim salesAccountLastModifiedDate As DateTime = CDate("1900-01-01 00:00:00")
                                                    If IsDBNull(dsTemp.Tables(0).Rows(0).Item("AccountLastModifiedDate1")) Then
                                                        salesAccountLastModifiedDate = CDate("1900-01-01 00:00:00")
                                                    Else
                                                        salesAccountLastModifiedDate = CDate(dsTemp.Tables(0).Rows(0).Item("AccountLastModifiedDate1"))
                                                    End If

                                                    Dim salesOppertunityLastModifiedDate As DateTime = CDate("1900-01-01 00:00:00")
                                                    If IsDBNull(dsTemp.Tables(0).Rows(0).Item("OpportunityLastModifiedDate")) Then
                                                        salesOppertunityLastModifiedDate = CDate("1900-01-01 00:00:00")
                                                    Else
                                                        salesOppertunityLastModifiedDate = CDate(dsTemp.Tables(0).Rows(0).Item("OpportunityLastModifiedDate"))
                                                    End If

                                                    If (salesAccountLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") <> AccountLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss")) OrElse (salesOppertunityLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") <> SalesforceLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss")) Then
                                                        canUpdate = True
                                                    End If
                                                Else
                                                    canUpdate = True
                                                End If


                                                If canUpdate Then
                                                    strQ = "select * from emp_CustomerInformation where SalesforceAccountID = '" & SalesforceAccountID & "' and SalesforceOpportunityID='" & SalesforceOppurtunityID & "'"
                                                    dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                                    Dim UpdatingCustomerId = CType(dsTemp.Tables(0).Rows(0).Item("Id"), System.Guid).ToString()
                                                    Dim OldEFIN As Integer = 0
                                                    If Not IsDBNull(dsTemp.Tables(0).Rows(0).Item("EFIN")) Then
                                                        OldEFIN = CType(dsTemp.Tables(0).Rows(0).Item("EFIN"), Integer)
                                                    End If
                                                    If Not IsDBNull(dsTemp.Tables(0).Rows(0).Item("ParentId")) Then
                                                        EMPParentID = CType(dsTemp.Tables(0).Rows(0).Item("ParentId"), Guid).ToString()
                                                    End If

                                                    If EMPParentID = "" Then
                                                        EMPParentID = "NULL"
                                                    Else
                                                        EMPParentID = "'" & EMPParentID & "'"
                                                    End If

                                                    If EMPParentID <> "NULL" And (EntityTypeID = 2 OrElse EntityTypeID = 3 OrElse EntityTypeID = 5 Or EntityTypeID = 9) Then
                                                        EMPParentID = "NULL"
                                                    End If

                                                    'If dsTemp.Tables(0).Rows(0).Item("CompanyName").ToString() <> CompanyName Then
                                                    '    CompanyName = dsTemp.Tables(0).Rows(0).Item("CompanyName").ToString()
                                                    'End If
                                                    'If dsTemp.Tables(0).Rows(0).Item("BusinesOwnerLastName").ToString() <> CompanyName Then
                                                    '    BusinessOwnerLName = dsTemp.Tables(0).Rows(0).Item("BusinesOwnerLastName").ToString()
                                                    'End If
                                                    'If dsTemp.Tables(0).Rows(0).Item("BusinessOwnerFirstName").ToString() <> CompanyName Then
                                                    '    BusinessOwnerFName = dsTemp.Tables(0).Rows(0).Item("BusinessOwnerFirstName").ToString()
                                                    'End If
                                                    'If dsTemp.Tables(0).Rows(0).Item("CompanyName").ToString() <> CompanyName Then
                                                    '    CompanyName = dsTemp.Tables(0).Rows(0).Item("CompanyName").ToString()
                                                    'End If


                                                    If OldEFIN <> EFIN Then
                                                        strQ = "select * from BankEnrollment where CustomerId='" & UpdatingCustomerId & "' and IsActive=1 and StatusCode in ('SUB','PEN','APR')"
                                                        dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                                        If dsTemp.Tables(0).Rows.Count > 0 Then
                                                            Continue For
                                                        End If
                                                    End If


                                                    strQ = "update emp_CustomerInformation set MasterIdentifier='" & masterIdentifier & "',EFIN=" & EFIN & ",Feeder='" & Feeder & "',BusinessOwnerFirstName='" & BusinessOwnerFName & "'
                                                            ,BusinesOwnerLastName='" & BusinessOwnerLName & "',CompanyName='" & CompanyName.Replace("'", "") & "',PrimaryEmail='" & PrimaryEmail & "',uTaxNotCollectingSBFee=" & IIf(buTax_Not_Collecting_SB_Fees__c, 1, 0) & "
                                                            ,Federal_EF_Fee_New__c='" & Quote_Federal_EF_Fee__c & "',State_EF_Fee_New__c='" & Quote_State_EF_Fee__c & "',TitleId=" & TitleId & ",OfficePhone='" & OfficePhone & "'
                                                            ,Type='" & OpportunityType & "',PhysicalAddress1='" & PhysicalAddress1.Replace("'", "") & "',PhysicalZipCode='" & PhysicalZipCode & "',PhysicalCity='" & PhysicalCity & "'
                                                            ,PhysicalState='" & PhysicalState & "',ShippingAddress1='" & ShippingAddress1.Replace("'", "") & "',ShippingZipCode='" & ShippingZipCode & "',ShippingCity='" & ShippingCity & "'
                                                            ,ShippingState='" & ShippingState & "',IsMSOUser=" & IIf(bMSO_User__c, 1, 0) & ",QuoteSoftwarePackage='" & Quote_Software_Package__c & "',Quote_Rebate__c=" & Quote_Rebate__c & " where Id='" & UpdatingCustomerId & "'"
                                                    'DBAccessDest.ExecuteQuery(strQ)

                                                    Dim EncryptedTransPassword As String = ""
                                                    If Not Transmission_Password__c <> "" Then
                                                        EncryptedTransPassword = PasswordManager.CryptText(Transmission_Password__c)
                                                    End If
                                                    strQ = "update emp_CustomerLoginInformation set MasterIdentifier='" & masterIdentifier & "',CrossLinkPassword='" & EncryptedTransPassword & "' where CustomerOfficeId='" & UpdatingCustomerId & "'"
                                                    DBAccessDest.ExecuteQuery(strQ)

                                                    Dim rootparentid As String = GetRootParent(EMPParentID, UpdatingCustomerId, False)
                                                    DBAccessDest.OfficeManagementGridSP(UpdatingCustomerId, SalesYearID, rootparentid)

                                                    strQ = "Select * from salesforce_sync_summary where salesforceOpportunityID = '" & SalesforceOppurtunityID & "' and SalesYear = '" & ActiveSalesYear & "'"
                                                    dsTemp = DBAccessDest.ExecuteQuery(strQ)
                                                    If dsTemp.Tables(0).Rows.Count > 0 Then
                                                        strQ = "update salesforce_sync_summary set Lastsync_timestamp=GETDATE(),OpportunityLastModifiedDate='" & SalesforceLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") & "',AccountLastModifiedDate1='" & AccountLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") & "',OpportunityLastActivityDate='" & SalesforceLastActivityDate.ToString("yyyy-MM-dd HH:mm:ss") & "',AccountLastActivityDate1='" & AccountLastActivityDate.ToString("yyyy-MM-dd HH:mm:ss") & "' where salesforceOpportunityID = '" & SalesforceOppurtunityID & "' and SalesYear = '" & ActiveSalesYear & "'"
                                                    Else
                                                        strQ = "insert into salesforce_sync_summary (sync_timestamp,salesforceOpportunityID,salesforceAccountID,EROType,OpportunityLastActivityDate,OpportunityLastModifiedDate,AccountLastActivityDate1,AccountLastModifiedDate1,emp_CustomerID,SalesYear,Lastsync_timestamp)
                                                         values (GETDATE(),'" & SalesforceOppurtunityID & "','" & SalesforceAccountID & "','" & EROType & "','" & SalesforceLastActivityDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & SalesforceLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & AccountLastActivityDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & AccountLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & UpdatingCustomerId & "','" & ActiveSalesYear & "',GETDATE())"
                                                    End If
                                                    DBAccessDest.ExecuteQuery(strQ)
                                                End If
#End Region
                                            End If
                                        End If
                                    End If
                                End If
                                Console.WriteLine(SalesforceOppurtunityID)
                            Catch ex As Exception
                                strQ = "insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" & ex.ToString().Replace("'", "") & " SalesforceAccountID :: " & SalesforceAccountID & "','" & Guid.Empty.ToString() & "','SyncService/SFDCtoEMP',GETDATE())"
                                DBAccessDest.ExecuteQuery(strQ)
                            End Try
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            Dim strQ As String = "insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" & ex.ToString().Replace("'", "") & "','" & Guid.Empty.ToString() & "','SyncService/SFDCtoEMP',GETDATE())"
            DBAccessDest.ExecuteQuery(strQ)
        End Try
    End Sub

    Private Sub SyncEMPdatatoSFDC()
        Dim strQ As String = "select top 1 * from SalesYearMaster where ApplicableToDate is null order by SalesYear desc"
        Dim ds As DataSet = DBAccessDest.ExecuteQuery(strQ)
        Dim ActiveSalesYear As String = ""
        Dim ActiveSalesYearId As String = ""
        If ds.Tables(0).Rows.Count > 0 Then
            ActiveSalesYear = CStr(ds.Tables(0).Rows(0).Item("SalesYear")).ToString()
            ActiveSalesYearId = CType(ds.Tables(0).Rows(0).Item("Id"), Guid).ToString()
        End If

        If ActiveSalesYear <> "" Then
            strQ = "select * from emp_CustomerInformation where StatusCode <> 'New' and SalesYearID='" & ActiveSalesYearId & "'"
            ds = DBAccessDest.ExecuteQuery(strQ)
            If ds.Tables(0).Rows.Count > 0 Then
                For Each dremp As DataRow In ds.Tables(0).Rows
                    Dim CustomerId As String = CType(dremp.Item("Id"), Guid).ToString()
                    Try

                        Dim EmpLastUpdatedDate As Date = CDate("1900-01-01 00:00:00")
                        If Not IsDBNull(dremp.Item("LastUpdatedDate")) Then
                            EmpLastUpdatedDate = CDate(dremp.Item("LastUpdatedDate"))
                        End If

                        Dim SalesforceAccountID As String = ""
                        If Not IsDBNull(dremp.Item("SalesforceAccountID")) Then
                            SalesforceAccountID = CStr(dremp.Item("SalesforceAccountID"))
                        End If

                        Dim SalesforceOpportunityID As String = ""
                        If Not IsDBNull(dremp.Item("SalesforceOpportunityID")) Then
                            SalesforceOpportunityID = CStr(dremp.Item("SalesforceOpportunityID"))
                        End If

                        If (SalesforceOpportunityID = "" And SalesforceAccountID = "") OrElse EmpLastUpdatedDate = CDate("1900-01-01 00:00:00") Then
                            Continue For
                        End If

                        Dim canUpdate As Boolean = False
                        strQ = "select * from salesforce_sync_summary where salesforceOpportunityID='" & SalesforceOpportunityID & "' and SalesYear='" & ActiveSalesYear & "'"
                        ds = DBAccessDest.ExecuteQuery(strQ)
                        If ds.Tables(0).Rows.Count > 0 Then
                            If Not IsDBNull(ds.Tables(0).Rows(0).Item("Emp_LastUpdatedDate")) Then
                                If EmpLastUpdatedDate.ToString("yyyy-MM-dd HH:mm:ss") <> DateTime.Parse(ds.Tables(0).Rows(0).Item("Emp_LastUpdatedDate")).ToString("yyyy-MM-dd HH:mm:ss") Then
                                    canUpdate = True
                                End If
                            Else
                                canUpdate = True
                            End If
                        Else
                            canUpdate = True
                        End If

                        If canUpdate = False Then
                            Continue For
                        End If

                        'Console.WriteLine(dremp.Item("CompanyName") & "  " & SalesforceAccountID)

                        Dim IsUpdated As Boolean = False
                        Dim SalesforceLastActivityDate As Date = CDate("1900-01-01 00:00:00")
                        Dim SalesforceLastModifiedDate As Date = CDate("1900-01-01 00:00:00")
                        Dim AccountLastActivityDate As Date = CDate("1900-01-01 00:00:00")
                        Dim AccountLastModifiedDate As Date = CDate("1900-01-01 00:00:00")

                        strQ = "select * from Account where ID = '" & SalesforceAccountID & "'"
                        Dim dsSF As DataSet = DBAccessSource.ExecuteQuery(strQ)
                        If dsSF.Tables(0).Rows.Count > 0 Then
                            If Not IsDBNull(dsSF.Tables(0).Rows(0).Item("LastActivityDate")) Then
                                AccountLastActivityDate = CDate(dsSF.Tables(0).Rows(0).Item("LastActivityDate"))
                            End If

                            If Not IsDBNull(dsSF.Tables(0).Rows(0).Item("LastModifiedDate")) Then
                                AccountLastModifiedDate = CDate(dsSF.Tables(0).Rows(0).Item("LastModifiedDate"))
                            End If

                            'strQ = "UPDATE OPENQUERY (Sandbox, 'SELECT Id,ShippingStreet FROM Account WHERE Id = ''" & SalesforceAccountID & "'' ')SET ShippingStreet = '" & dremp.Item("ShippingAddress1").ToString() & "';" '"update " & MyConfig.LinkedDBName & "Account set"
                            'Dim strqCom = strQ

                            'If Not IsDBNull(dsSF.Tables(0).Rows(0).Item("EFIN__c")) Then
                            '    If CType(dremp.Item("EFIN"), Integer).ToString() <> CStr(dsSF.Tables(0).Rows(0).Item("EFIN__c")) Then
                            '        strQ += " EFIN__c=" & CType(dremp.Item("EFIN"), Integer).ToString()
                            '    End If
                            'Else
                            '    strQ += " EFIN__c=" & CType(dremp.Item("EFIN"), Integer).ToString()
                            'End If

                            'Dim IsFeeder As String = ""
                            'Dim strQ1 As String = "select * from SubSiteOfficeConfig where RefId ='" & CustomerId & "'"
                            'ds = DBAccessDest.ExecuteQuery(strQ1)
                            'If ds.Tables(0).Rows.Count > 0 Then
                            '    If Not IsDBNull(ds.Tables(0).Rows(0).Item("SubSiteSendTaxReturn")) Then
                            '        IsFeeder = CType(ds.Tables(0).Rows(0).Item("SubSiteSendTaxReturn"), Boolean).ToString().ToLower()
                            '    End If
                            'End If
                            'If IsFeeder <> "" Then
                            '    strQ += If(strQ = strqCom, " Feeder_Sites__c='" & IsFeeder & "'", ",Feeder_Sites__c='" & IsFeeder & "'")
                            'End If

                            'strQ += If(strQ = strqCom, " Name='" & dremp.Item("CompanyName").ToString() & "'", ",Name='" & dremp.Item("CompanyName").ToString() & "'")
                            'strQ += ",BillingStreet='" & dremp.Item("PhysicalAddress1").ToString() & "'"
                            'strQ += ",BillingCity='" & dremp.Item("PhysicalCity").ToString() & "'"
                            'strQ += ",BillingState='" & dremp.Item("PhysicalState").ToString() & "'"
                            'strQ += ",BillingPostalCode='" & dremp.Item("PhysicalZipCode").ToString() & "'"
                            'strQ += ",ShippingStreet='" & dremp.Item("ShippingAddress1").ToString() & "'"
                            'strQ += ",ShippingCIty='" & dremp.Item("ShippingCity").ToString() & "'"
                            'strQ += ",ShippingState='" & dremp.Item("ShippingState").ToString() & "'"
                            'strQ += ",ShippingPostalCode='" & dremp.Item("ShippingZipCode").ToString() & "'"

                            If Not IsDBNull(dsSF.Tables(0).Rows(0).Item("EFIN__c")) Then
                                If CType(dremp.Item("EFIN"), Integer).ToString() <> CStr(dsSF.Tables(0).Rows(0).Item("EFIN__c")) Then
                                    DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "EFIN__c", "" & CType(dremp.Item("EFIN"), Integer).ToString() & "")
                                End If
                            Else
                                DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "EFIN__c", "" & CType(dremp.Item("EFIN"), Integer).ToString() & "")
                            End If

                            Dim IsFeeder As String = ""
                            Dim strQ1 As String = "select * from SubSiteOfficeConfig where RefId ='" & CustomerId & "'"
                            ds = DBAccessDest.ExecuteQuery(strQ1)
                            If ds.Tables(0).Rows.Count > 0 Then
                                If Not IsDBNull(ds.Tables(0).Rows(0).Item("SubSiteSendTaxReturn")) Then
                                    IsFeeder = CType(ds.Tables(0).Rows(0).Item("SubSiteSendTaxReturn"), Boolean).ToString().ToLower()
                                End If
                            End If
                            If IsFeeder <> "" Then
                                If IsFeeder = "true" Then
                                    DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "Feeder_Sites__c", "1")
                                Else
                                    DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "Feeder_Sites__c", "0")
                                End If

                            End If

                            DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "Name", "'" & dremp.Item("CompanyName").ToString() & "'")
                            DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "BillingStreet", "'" & dremp.Item("PhysicalAddress1").ToString() & "'")
                            DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "BillingCity", "'" & dremp.Item("PhysicalCity").ToString() & "'")
                            DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "BillingState", "'" & dremp.Item("PhysicalState").ToString() & "'")
                            DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "BillingPostalCode", "'" & dremp.Item("PhysicalZipCode").ToString() & "'")
                            If dremp.Item("ShippingAddress1").ToString() <> "" Then
                                DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "ShippingStreet", "'" & dremp.Item("ShippingAddress1").ToString() & "'")
                            End If
                            If dremp.Item("ShippingCity").ToString() <> "" Then
                                DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "ShippingCIty", "'" & dremp.Item("ShippingCity").ToString() & "'")
                            End If
                            If dremp.Item("ShippingState").ToString() <> "" Then
                                DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "ShippingState", "'" & dremp.Item("ShippingState").ToString() & "'")
                            End If
                            If dremp.Item("ShippingZipCode").ToString() <> "" Then
                                DBAccessSource.InvokeSP_UpdateSFAccount(SalesforceAccountID, "ShippingPostalCode", "'" & dremp.Item("ShippingZipCode").ToString() & "'")
                            End If


                            'strQ += " where ID = '" & SalesforceAccountID & "'"
                            'Console.WriteLine(strQ)
                            'DBAccessSource.ExecuteQuery(strQ)
                            'Console.ReadLine()
                            IsUpdated = True
                        End If

                        strQ = "select * from Contact where AccountId ='" & SalesforceAccountID & "' and Primary_Contact__c= 'true'"
                        dsSF = DBAccessSource.ExecuteQuery(strQ)
                        If dsSF.Tables(0).Rows.Count > 0 Then

                            Dim SalesforceContactId As String = dsSF.Tables(0).Rows(0).Item("Id").ToString()
                            DBAccessSource.InvokeSP_UpdateSFContact(SalesforceContactId, "FirstName", "'" & dremp.Item("BusinessOwnerFirstName").ToString() & "'")
                            DBAccessSource.InvokeSP_UpdateSFContact(SalesforceContactId, "LastName", "'" & dremp.Item("BusinesOwnerLastName").ToString() & "'")
                            If dremp.Item("OfficePhone").ToString() <> "" Then
                                DBAccessSource.InvokeSP_UpdateSFContact(SalesforceContactId, "Phone", "'" & dremp.Item("OfficePhone").ToString() & "'")
                            End If
                            If dremp.Item("PrimaryEmail").ToString() <> "" Then
                                DBAccessSource.InvokeSP_UpdateSFContact(SalesforceContactId, "Email", "'" & dremp.Item("PrimaryEmail").ToString().Replace("'", "") & "'")
                            End If


                            If Not IsDBNull(dremp.Item("TitleId")) Then
                                Dim title As String = CType(dremp.Item("TitleId"), Guid).ToString()
                                Dim strQ1 As String = "select * from ContactPersonTitleMaster where Id = '" & title & "'"
                                ds = DBAccessDest.ExecuteQuery(strQ1)
                                If ds.Tables(0).Rows.Count > 0 Then
                                    DBAccessSource.InvokeSP_UpdateSFContact(SalesforceContactId, "Title", "'" & ds.Tables(0).Rows(0).Item("ContactPersonTitle").ToString() & "'")
                                End If
                            End If

                            'strQ = "update " & MyConfig.LinkedDBName & "Contact set"
                            'Dim strQCom As String = "update " & MyConfig.LinkedDBName & "Contact set"

                            'strQ += " FirstName='" & dremp.Item("BusinessOwnerFirstName").ToString() & "'"
                            'strQ += ",LastName='" & dremp.Item("BusinesOwnerLastName").ToString() & "'"
                            'strQ += ",Email='" & dremp.Item("PrimaryEmail").ToString() & "'"
                            'strQ += ",Phone='" & dremp.Item("OfficePhone").ToString() & "'"
                            'strQ += ",Title ='" & ds.Tables(0).Rows(0).Item("ContactPersonTitle") & "'"
                            'strQ += " where AccountId ='" & SalesforceAccountID & "' and Primary_Contact__c= 'true'"
                            'DBAccessSource.ExecuteQuery(strQ)
                            IsUpdated = True
                        End If

                        strQ = "select * from Opportunity where Id='" & SalesforceOpportunityID & "'"
                        dsSF = DBAccessSource.ExecuteQuery(strQ)
                        If dsSF.Tables(0).Rows.Count > 0 Then
                            Dim TechFee As Decimal = 0
                            Dim TransFee As Decimal = 0
                            strQ = "select * from FeeMaster where FeesFor = 1 and FeeTypeId =1"
                            ds = DBAccessDest.ExecuteQuery(strQ)
                            If ds.Tables(0).Rows.Count > 0 Then
                                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Amount")) Then
                                    TechFee = CType(ds.Tables(0).Rows(0).Item("Amount"), Decimal)
                                    'Console.WriteLine("TechFee" & TechFee)
                                    DBAccessSource.InvokeSP_UpdateSFOpportunity(SalesforceOpportunityID, "Technology_Fee__c", TechFee)
                                    'Console.WriteLine("TechFee Done")
                                End If
                            End If
                            strQ = "select * from FeeMaster where FeesFor = 3"
                            ds = DBAccessDest.ExecuteQuery(strQ)
                            If ds.Tables(0).Rows.Count > 0 Then
                                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Amount")) Then
                                    TransFee = CType(ds.Tables(0).Rows(0).Item("Amount"), Decimal)
                                    'Console.WriteLine("TransFee" & TransFee)
                                    DBAccessSource.InvokeSP_UpdateSFOpportunity(SalesforceOpportunityID, "Transmitter_Fee__c", TransFee)
                                End If
                            End If

                        End If

                        If IsUpdated Then
                            strQ = "select * from salesforce_sync_summary where salesforceOpportunityID='" & SalesforceOpportunityID & "' and SalesYear='" & ActiveSalesYear & "'"
                            ds = DBAccessDest.ExecuteQuery(strQ)
                            If ds.Tables(0).Rows.Count > 0 Then
                                strQ = "update salesforce_sync_summary set Emp_LastUpdatedDate = '" & EmpLastUpdatedDate & "',IsEMPtoSFDC=1 where salesforceOpportunityID='" & SalesforceOpportunityID & "' and SalesYear='" & ActiveSalesYear & "'"
                                DBAccessDest.ExecuteQuery(strQ)
                            Else
                                strQ = "insert into salesforce_sync_summary (sync_timestamp,salesforceOpportunityID,salesforceAccountID,EROType,OpportunityLastActivityDate,OpportunityLastModifiedDate,AccountLastActivityDate1,AccountLastModifiedDate1,emp_CustomerID,SalesYear,Lastsync_timestamp,IsEMPtoSFDC,Emp_LastUpdatedDate)
                                   values (GETDATE(),'" & SalesforceOpportunityID & "','" & SalesforceAccountID & "','" & dremp.Item("EROType") & "','" & AccountLastActivityDate.ToString("yyyy-MM-dd") & "','" & AccountLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") & "',
                                   '" & AccountLastActivityDate.ToString("yyyy-MM-dd") & "','" & AccountLastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss") & "','" & CustomerId & "','" & ActiveSalesYear & "',GETDATE(),1,'" & EmpLastUpdatedDate.ToString("yyyy-MM-dd HH:mm:ss") & "')"
                                DBAccessDest.ExecuteQuery(strQ)
                            End If
                        End If

                    Catch ex As Exception
                        strQ = "insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" & ex.ToString().Replace("'", "") & " for customer :: " & CustomerId & "','" & Guid.Empty.ToString() & "','SyncService/EMPtoSFDB',GETDATE())"
                        DBAccessDest.ExecuteQuery(strQ)
                    End Try
                Next
            End If
        End If
    End Sub

    Private Function GetRootParent(ByVal EMPParentID As String, ByVal NewCustomerID As String, ByVal Optional IsInsert As Boolean = True) As String

        Dim ParentId As String = EMPParentID
        Dim rootparentid As String = ParentId
        Dim isRoot As Boolean = True
        Dim HierarchyCount As Integer = 1
        Dim strQH As String = ""


        'Console.WriteLine(EMPParentID & "  " & NewCustomerID)

        If ParentId Is Nothing OrElse EMPParentID Is Nothing Then
            Return Nothing
        End If


        'If Not String.IsNullOrEmpty(ParentId) And Not ParentId Is Nothing And EMPParentID <> "NULL" Then
        While isRoot
            Dim dt1 As DataTable = DBAccessDest.ExecuteQuery("select * from emp_CustomerInformation where Id = " & rootparentid & "").Tables(0)
            If dt1.Rows.Count > 0 Then
                If IsInsert Then
                    strQH = "Insert into EntityHierarchy (RelationId, Customer_Level, CustomerId, EntityID, Status) values ('" &
                                                                    NewCustomerID & "', " & HierarchyCount & ", " & rootparentid & ", " & dt1.Rows(0)("EntityId") & ", 'ACT')"
                    DBAccessDest.ExecuteQuery(strQH)
                    HierarchyCount = HierarchyCount + 1
                End If
                If String.IsNullOrEmpty(dt1.Rows(0)("ParentId").ToString()) Then
                    isRoot = False
                Else
                    Dim rp As Guid = CType(dt1.Rows(0)("ParentId"), Guid)
                    If rp = Guid.Empty Then
                        isRoot = False
                    Else
                        rootparentid = dt1.Rows(0)("ParentId").ToString()
                    End If
                End If
            Else
                isRoot = False
            End If
        End While
        'End If

        If String.IsNullOrEmpty(rootparentid) OrElse rootparentid = "NULL" Then
            rootparentid = Nothing
        End If
        Return rootparentid

    End Function

End Module
