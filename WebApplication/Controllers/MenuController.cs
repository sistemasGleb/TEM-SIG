using BusinessImpl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class MenuController : BaseController
    {
        private readonly MenuBusinessImpl menuBusinessImpl = new MenuBusinessImpl();

        [Authorize]
        public ActionResult MenuOpciones()
        {
            BusinessEntity.MenuBusinessEntity.MenuUsuario menuUsuario = new BusinessEntity.MenuBusinessEntity.MenuUsuario();

            menuUsuario.menu_opciones = new List<BusinessEntity.MenuBusinessEntity.MenuOpciones>();

            try
            {

                var dataSetSQL = menuBusinessImpl.fncDevolverMenuOpciones(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 1) - EXTRAE LOS NODOS PADRE
                var listLevel1 = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                            .Where(m => (Int16)m["mum_level_id"] == 1)
                                            .Select(m => new BusinessEntity.MenuBusinessEntity.UrlMenu
                                            {
                                                mum_id = (Int32)m["mum_id"],
                                                mum_root_id = (Int16)m["mum_root_id"],
                                                mum_level_id = (Int16)m["mum_level_id"],
                                                mum_order_id = (Int16)m["mum_order_id"],
                                                mum_area = m["mum_area"] == DBNull.Value ? null : (string)m["mum_area"],
                                                mum_controller = m["mum_controller"] == DBNull.Value ? null : (string)m["mum_controller"],
                                                mum_method = m["mum_method"] == DBNull.Value ? null : (string)m["mum_method"],
                                                mum_icon = m["mum_icon"] == DBNull.Value ? null : (string)m["mum_icon"],
                                                mum_caption = m["mum_caption"] == DBNull.Value ? null : (string)m["mum_caption"],
                                            }).OrderBy(X => X.mum_order_id).ToList();

                // PASO ) - EXTRAE LOS NODOS HIJO
                var listLevel2 = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                            .Where(m => (Int16)m["mum_level_id"] == 2)
                                            .Select(m => new BusinessEntity.MenuBusinessEntity.UrlMenu
                                            {
                                                mum_id = (Int32)m["mum_id"],
                                                mum_root_id = (Int16)m["mum_root_id"],
                                                mum_level_id = (Int16)m["mum_level_id"],
                                                mum_order_id = (Int16)m["mum_order_id"],
                                                mum_area = m["mum_area"] == DBNull.Value ? null : (string)m["mum_area"],
                                                mum_controller = m["mum_controller"] == DBNull.Value ? null : (string)m["mum_controller"],
                                                mum_method = m["mum_method"] == DBNull.Value ? null : (string)m["mum_method"],
                                                mum_icon = m["mum_icon"] == DBNull.Value ? null : (string)m["mum_icon"],
                                                mum_caption = m["mum_caption"] == DBNull.Value ? null : (string)m["mum_caption"],
                                            }).OrderBy(X => X.mum_order_id).ToList();
 

                // PASO 3) - AÑADIMOS LOS NODOS CON ORDEN 
                foreach (var url in listLevel1)
                {
                    BusinessEntity.MenuBusinessEntity.MenuOpciones _menu = new BusinessEntity.MenuBusinessEntity.MenuOpciones();

                    _menu.mum_id = url.mum_id;
                    _menu.mum_root_id = url.mum_root_id;
                    _menu.mum_level_id = url.mum_level_id;
                    _menu.mum_order_id = url.mum_order_id;
                    _menu.mum_emp_id = url.mum_emp_id;
                    _menu.mum_area = url.mum_area;
                    _menu.mum_controller = url.mum_controller;
                    _menu.mum_method = url.mum_method;
                    _menu.mum_view = url.mum_view;
                    _menu.mum_icon = url.mum_icon;
                    _menu.mum_caption = url.mum_caption;
                    _menu.mum_vig = url.mum_vig;
                    _menu.child = listLevel2.Where(x => x.mum_level_id == 2 && x.mum_root_id == url.mum_id).OrderBy(x => x.mum_order_id).ToList();

                    menuUsuario.menu_opciones.Add(_menu);
                }
 
            }
            catch
            {
                menuUsuario.menu_opciones = new List<BusinessEntity.MenuBusinessEntity.MenuOpciones>();
            }

            return PartialView("_Opciones", menuUsuario);
        }
    }
}
